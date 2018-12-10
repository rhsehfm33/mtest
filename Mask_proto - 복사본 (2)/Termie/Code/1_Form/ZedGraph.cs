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
    public partial class MainForm
    {
        public enum PacketDataType
        {
            eBreath,
            ePressure,
            eLRPM,
            eRRPM,
            eEND
        }
        PointPairList[] Datalist = new PointPairList[(int)PacketDataType.eEND];

        double zoomFraction = 1.0;

        void BreathGraph_MouseWheel(object sender, MouseEventArgs e)
        {
            double newZoomFraction = zoomFraction + (e.Delta < 0 ? 1.0 : -1.0) * BreathGraph.ZoomStepFraction;
            if (newZoomFraction > 0.0001 && newZoomFraction < 1000.0)
            {
                zoomFraction = newZoomFraction;
                if (Datalist[0].Count > 0)
                {
                    BreathGraph.GraphPane.XAxis.Scale.Max = (Datalist[0])[Datalist[0].Count - 1].X;
                }
            }
        }

        public void drawzed(RealPacket packet)
        {
            // test
            float fTime = (float)m_sw.ElapsedMilliseconds / 1000.0F;
            Random r = new Random();

            Datalist[0].Add(fTime, packet.breath);
            Datalist[1].Add(fTime, packet.pressure);
            Datalist[2].Add(fTime, packet.LRPM);
            Datalist[3].Add(fTime, packet.RRPM);

            BreathGraph.GraphPane.AddCurve("", Datalist[0], Color.Orange, SymbolType.None);
            BreathGraph.GraphPane.XAxis.Scale.Max = fTime;
            BreathGraph.GraphPane.XAxis.Scale.Min = fTime - (8.0 * zoomFraction);
            BreathGraph.AxisChange();

            PressureGraph.GraphPane.AddCurve("", Datalist[1], Color.Green, SymbolType.None);
            PressureGraph.GraphPane.XAxis.Scale.Max = fTime;
            PressureGraph.GraphPane.XAxis.Scale.Min = fTime - (8.0 * zoomFraction);
            PressureGraph.AxisChange();

            RPMGraph.GraphPane.AddCurve("L", Datalist[2], Color.Blue, SymbolType.None);
            RPMGraph.GraphPane.AddCurve("R", Datalist[3], Color.Red, SymbolType.None);
            RPMGraph.GraphPane.XAxis.Scale.Max = fTime;
            RPMGraph.GraphPane.XAxis.Scale.Min = fTime - (8.0 * zoomFraction);
            RPMGraph.AxisChange();
        }
        RectangleF a;
        public void InitializeGraph()
        {
            a = BreathGraph.GraphPane.Rect;
            InitializeGraph(BreathGraph);
            InitializeGraph(RPMGraph);
            InitializeGraph(PressureGraph);

            SetChartPosition();

            Datalist[(int)PacketDataType.eBreath] = new PointPairList();
            Datalist[(int)PacketDataType.eLRPM] = new PointPairList();
            Datalist[(int)PacketDataType.eRRPM] = new PointPairList();
            Datalist[(int)PacketDataType.ePressure] = new PointPairList();

        }

        private void SetChartPosition()
        {
            RectangleF rect = new RectangleF();

            float Xposition = (BreathGraph.GraphPane.Chart.Rect.X + RPMGraph.GraphPane.Chart.Rect.X) * 0.5F;
            float Xwidth = (BreathGraph.GraphPane.Chart.Rect.Width + RPMGraph.GraphPane.Chart.Rect.Width) * 0.5F;

            rect = RPMGraph.GraphPane.Chart.Rect;
            rect.X = Xposition;
            rect.Width = Xwidth;
            RPMGraph.GraphPane.Chart.Rect = rect;

            rect = PressureGraph.GraphPane.Chart.Rect;
            rect.X = Xposition;
            rect.Width = Xwidth;
            PressureGraph.GraphPane.Chart.Rect = rect;
            PressureGraph.GraphPane.Chart.Rect = rect;

            rect = BreathGraph.GraphPane.Chart.Rect;
            rect.X = Xposition;
            rect.Width = Xwidth;
            BreathGraph.GraphPane.Chart.Rect = rect;
            BreathGraph.GraphPane.Chart.Rect = rect;

            BreathGraph.GraphPane.Chart.IsRectAuto = false;
            PressureGraph.GraphPane.Chart.IsRectAuto = false;
            RPMGraph.GraphPane.Chart.IsRectAuto = false;
        }
        public void InitializeGraph(ZedGraph.ZedGraphControl graph)
        {
            graph.MouseWheel += BreathGraph_MouseWheel;
            GraphPane myPane = graph.GraphPane;

            graph.IsShowPointValues = true;           // Mouse over시 data 표시
            graph.PointValueFormat = "G";

            myPane.Fill.Color = System.Drawing.Color.WhiteSmoke;
            myPane.Legend.IsVisible = false;                                        // Legend
            myPane.Border.IsVisible = false;
            myPane.Title.IsVisible = false;

            myPane.XAxis.Scale.Format = "0.000";
            myPane.XAxis.Scale.MajorStep = 1.000;
            myPane.YAxis.Scale.Format = "0.0";

            myPane.XAxis.Scale.Max = 4.0;                  //  x축 최대치
            myPane.XAxis.Scale.Min = -4.0;
            myPane.YAxis.Scale.Max = 10.0;
            myPane.YAxis.Scale.Min = -10.0;
            
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.XAxis.MajorGrid.Color = System.Drawing.Color.LightGray;
            myPane.XAxis.MajorGrid.DashOff = 0.0F;
            myPane.XAxis.MinorGrid.IsVisible = true;
            myPane.XAxis.MinorGrid.DashOn = 4.0F;
            myPane.XAxis.MinorGrid.Color = System.Drawing.Color.LightGray;
            myPane.YAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.Color = System.Drawing.Color.LightGray;
            myPane.YAxis.MajorGrid.DashOff = 0.0F;

            myPane.XAxis.Title.IsVisible = false;
            myPane.YAxis.Title.IsVisible = false;

            graph.AxisChange();
            graph.Invalidate();

            myPane.YAxis.Scale.MaxAuto = true;
            myPane.YAxis.Scale.MinAuto = true;
        }

        private void ResetGraph()
        {
            foreach (PointPairList list in Datalist)
                if(list != null)
                    list.Clear();
        }
    }
}
