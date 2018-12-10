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
using ZedGraph;

namespace Termie
{
    public partial class MainForm : Form
    {
        #region Variable
        Stopwatch m_sw;
        Stopwatch m_sw2;
        #endregion

        #region 민성
        bool _bLogging;
        #endregion
        #region Default
        public MainForm()
        {
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            InitializeComponent();
            _bLogging = false;
            Settings.Read();
            //Settings.setLogPathFile(ref this.LogPathBox);
            TopMost = Settings.Option.StayOnTop;

            CommPort com = CommPort.Instance;
            com.StatusChanged += OnStatusChanged;
            com.DataReceived += OnDataReceived;
            com.Open();

            int[] Interval = { 1, 10, 100, 200, 1000, 0 };
            for (int i = 0; Interval[i] != 0; ++i)
                IntervalComboBox.Items.Add(Interval[i].ToString());
            IntervalComboBox.SelectedIndex = 1;

            // stopwatch
            m_sw = new Stopwatch();
            m_sw.Reset();
            m_sw.Start();
            m_sw2 = new Stopwatch();
            m_sw2.Reset();
            m_sw2.Start();

            InitializeGraph();
        }

        // shutdown the worker thread when the form closes
        protected override void OnClosed(EventArgs e)
        {
            CommPort com = CommPort.Instance;
            com.Close();

            Settings.Write();

            base.OnClosed(e);
        }
        #endregion

        #region Event handling - data received and status changed

        // delegate used for Invoke
        internal delegate void StringDelegate(string data);
        internal delegate void PacketDelegate(RealPacket data);

        /// <summary>
        /// Handle data received event from serial port.
        /// </summary>
        /// <param name="data">incoming data</param>
        public void OnDataReceived(RealPacket dataIn)
        {
            //Handle multi-threading
            if (InvokeRequired)
            {
                Invoke(new PacketDelegate(OnDataReceived), new object[] { dataIn });
                return;
            }

            // Read Data
            //DrawGraph(dataIn);          // 그래프는 대기
            if (m_sw2.ElapsedMilliseconds > 50)
            {
                m_sw2.Restart();
                drawzed(dataIn);
                BreathGraph.Invalidate();
                PressureGraph.Invalidate();
                RPMGraph.Invalidate();
                dataGridView.Invalidate();

            }
            if (_bLogging)
            {
                DrawGrid(dataIn);
            }
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
        public void Write_ExcelData()
        {
            Excel.Application excelApp = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;
            object missing = System.Reflection.Missing.Value;

            try
            {
                int r = 1, c = 1;
                // Excel 첫번째 워크시트 마지막 번째 가져오기. lms 추가함.
                excelApp = new Excel.Application();
                wb = excelApp.Workbooks.Add(missing);
                ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;


                for (; c <= dataGridView.ColumnCount; c++)
                {
                    ws.Cells[r, c] = dataGridView.Columns[c - 1].HeaderText;
                }
                for (r = 2; r <= dataGridView.Rows.Count + 1; r++)
                {
                    for (c = 1; c <= dataGridView.ColumnCount; c++)
                    {
                        ws.Cells[r, c] = dataGridView.Rows[r - 2].Cells[c - 1].Value;
                    }
                }
                wb.SaveAs(Settings.Option.LogFilePath);
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

            // RPMGraph.Series[0].Points.AddXY(fTime, r.NextDouble() * 10.0F - 5.0F);
            //PressureGraph.Series[0].Points.AddXY(fTime, r.NextDouble() * 10.0F - 5.0F);

            //BreathGraph.Invalidate();
            // RPMGraph.Invalidate();
            // PressureGraph.Invalidate();
            // BreathGraph.Series[0].Points.AddXY(tmp, packet.m_DataIn[(int)PacketDataType.eBreath]);
            // RPMGraph.Series[0].Points.AddXY(tmp, packet.m_DataIn[(int)PacketDataType.eRPM]);
            // PressureGraph.Series[0].Points.AddXY(tmp, packet.m_DataIn[(int)PacketDataType.ePressure]);

        }
        #endregion

        #region Ghong

        #region Button
        private void StartButton_Click(object sender, EventArgs e)
        {
            CommPort com = CommPort.Instance;
            if (!com.IsRunning())
            {
                ResetGraph();
                com.Button_Click();
                StartButton.Enabled = false;
                StopButton.Enabled = true;
                // BreathGraph.GraphPane.XAxis.Scale.MinAuto = true;
            }

        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            CommPort com = CommPort.Instance;
            if (com.IsRunning())
            {
                com.Button_Click();
                StartButton.Enabled = true;
                StopButton.Enabled = false;
            }
        }

        private void SettingButton_Click(object sender, EventArgs e)
        {
            SettingForm settingform = new SettingForm();
            settingform.ShowDialog();
        }
        #endregion

        #endregion

        #region MinSeong        // 수정 필요.
        private void btnLogStart_Click(object sender, EventArgs e)
        {
            _bLogging = !_bLogging;
            if (_bLogging)
                btnLogStart.Text = "Pause";
            else
                btnLogStart.Text = "Start";
        }

        public void DrawGrid(RealPacket packet)
        {
            //float fTime = (float)m_sw.ElapsedMilliseconds / 1000.0F;
            dataGridView.Rows.Add(packet.breath, packet.pressure, packet.LRPM, packet.RRPM);
            dataGridView.FirstDisplayedScrollingRowIndex = dataGridView.RowCount - 1;

        }

        private void btnLogStop_Click(object sender, EventArgs e)
        {
            Write_ExcelData();
        }

        private void btnLogPath_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog1 = new SaveFileDialog();

            fileDialog1.Title = "Save Log As";
            fileDialog1.Filter = "Excel files (*.xlsx)|*.xlsx";
            fileDialog1.FilterIndex = 1;
            fileDialog1.RestoreDirectory = true;

            if (fileDialog1.ShowDialog() == DialogResult.OK)
            {
                button5.Text = fileDialog1.FileName;
                Settings.Option.LogFilePath = fileDialog1.FileName;
                if (File.Exists(button5.Text))
                    File.Delete(button5.Text);
            }
            else
            {
                button5.Text = "";
            }
        }
        #endregion
    }
}
