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
        bool bRunning = true;

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

            long a = m_sw.ElapsedMilliseconds;

            // Read Data
            DrawGraph(dataIn);          // 그래프는 대기

            // Logging                 // 로깅부분 작업 - 민성
            //if (bLogging)
            //{
            //    DrawGrid();
            //}
            //#region "ReGion Name"
            //
            //ddddddd
            //#endregion
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
            Random r = new Random();

            for (int i = 0; i < 60; ++i)
            {
                string tmp = ((int)fTime).ToString() + '.' + i.ToString();

                BreathGraph.Series[0].Points.AddXY(tmp, r.NextDouble() * 10.0F - 5.0F);
                RPMGraph.Series[0].Points.AddXY(tmp, r.NextDouble() * 10.0F - 5.0F);
                PressureGraph.Series[0].Points.AddXY(tmp, r.NextDouble() * 10.0F - 5.0F);

                BreathGraph.Invalidate();
                RPMGraph.Invalidate();
                PressureGraph.Invalidate();
                // BreathGraph.Series[0].Points.AddXY(tmp, packet.m_DataIn[(int)PacketDataType.eBreath]);
                // RPMGraph.Series[0].Points.AddXY(tmp, packet.m_DataIn[(int)PacketDataType.eRPM]);
                // PressureGraph.Series[0].Points.AddXY(tmp, packet.m_DataIn[(int)PacketDataType.ePressure]);
            }
            

            
        }
        #endregion

        private void button6_Click(object sender, EventArgs e)
        {
            Packet pac = new Packet();

            Random r = new Random();

            pac.m_DataIn[0] = (float)r.NextDouble() * 10.0F;
            pac.m_DataIn[1] = (float)r.NextDouble() * 10.0F;
            pac.m_DataIn[2] = (float)r.NextDouble() * 10.0F;

            DrawGraph(pac);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bRunning = !bRunning;
        }




        #region Ghong

        #endregion

        #region MinSeong

        #endregion
    }
}
