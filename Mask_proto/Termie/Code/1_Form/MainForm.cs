using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace Termie
{
    public partial class MainForm : Form
    {
        #region Variable
        bool bRunning = false;

        Stopwatch m_sw;
        #endregion

        #region Default
        public MainForm()
        {
            InitializeComponent();

            Settings.Read();
            TopMost = Settings.Option.StayOnTop;

            CommPort com = CommPort.Instance;
            com.StatusChanged += OnStatusChanged;
            com.DataReceived += OnDataReceived;
            com.Open();

            // stopwatch
            m_sw = new Stopwatch();
            m_sw.Reset();
            m_sw.Start();
        }

        // shutdown the worker thread when the form closes
        protected override void OnClosed(EventArgs e)
        {
            CommPort com = CommPort.Instance;
            com.Close();

            base.OnClosed(e);
        }
        #endregion

        #region Event handling - data received and status changed

        // delegate used for Invoke
        internal delegate void StringDelegate(string data);
        internal delegate void PacketDelegate(Packet data);

        /// <summary>
        /// Handle data received event from serial port.
        /// </summary>
        /// <param name="data">incoming data</param>
        public void OnDataReceived(Packet dataIn)
        {
            //Handle multi-threading
            if (InvokeRequired)
            {
                Invoke(new PacketDelegate(OnDataReceived), new object[] { dataIn });
                return;
            }

            // Read Data
            if(bRunning)
                DrawGraph(dataIn);
        }

        /// <summary>
        /// Update the connection status
        /// </summary>
        public void OnStatusChanged(string status)
        {
            //Handle multi-threading
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnStatusChanged), new object[] { status });
                return;
            }

            textBox1.Text = status;
        }

        #endregion

        #region Excel
        public void Write_ExcelData(string stringout)
        {
            Excel.Application excelApp = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;

            try
            {
                int r = 1, c = 1;
                // Excel 첫번째 워크시트 마지막 번째 가져오기. lms 추가함.
                excelApp = new Excel.Application();
                wb = excelApp.Workbooks.Open(Settings.Option.LogFilePath);
                ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;

                Excel.Range last = ws.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
                r = last.Row;
                string num = "";

                foreach (char n in stringout)
                {
                    if (n == Token.seriesToken)
                    {
                        ws.Cells[r, c] = float.Parse(num);
                        num = "";
                        c++;
                    }
                    else if (n == Token.lineToken)
                    {
                        num = "";
                        r++;
                        c = 1;
                    }
                    else
                    {
                        num += n;
                    }
                }
                wb.Save();
                wb.Close(true);
                excelApp.Quit();
            }
            finally
            {
                // Clean up
                ReleaseExcelObject(ws);
                ReleaseExcelObject(wb);
                ReleaseExcelObject(excelApp);
            }
        }
        private static void ReleaseExcelObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }
        #endregion

        #region Graph Control
        public void DrawGraph(Packet packet)
        {
            float fTime = (float)m_sw.ElapsedMilliseconds / 1000.0F;

            BreathGraph.Series[0].Points.AddXY(fTime, packet.m_DataIn[(int)PacketDataType.eBreath]);

            //BreathGraph.Series[0].Points.AddXY(


            //int series = 0;
            //string num = "";        // 엑셀로부터 읽은 하나의 숫자 값(float). 
            //foreach (char c in str)
            //{
            //    if (c == Token.seriesToken)
            //    {    // 라인 문자일 경우 line 변수를 올려서 시리즈의 인덱스 값 증가시킴.
            //        graph.Series[series].Points.AddXY(autoInc, float.Parse(num)); //온전히 한 셀에 숫자를 다 입력받으면 그래프에 입력.
            //        num = "";
            //        series++;
            //    }
            //    else if (c == Token.lineToken)
            //    {
            //        series = 0;
            //        autoInc++;
            //    }
            //    else
            //    {
            //        num += c;
            //    }
            //}
        }
        #endregion

        #region Ghong

        #endregion

        #region MinSeong
        
        #endregion
    }
}
