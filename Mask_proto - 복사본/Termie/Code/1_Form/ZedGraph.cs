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
            eRPM,
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

        public void drawzed(Packet packet)
        {
            // test
            float fTime = (float)m_sw.ElapsedMilliseconds / 1000.0F;
            Random r = new Random();

            foreach(PointPairList list in Datalist)
                list.Add(fTime, (r.NextDouble() * 10 - 5));

            BreathGraph.GraphPane.AddCurve("", Datalist[0], Color.Red, SymbolType.None);
            BreathGraph.GraphPane.XAxis.Scale.Max = fTime;
            BreathGraph.GraphPane.XAxis.Scale.Min = fTime - (8.0 * zoomFraction);
            BreathGraph.AxisChange();

            RPMGraph.GraphPane.AddCurve("", Datalist[0], Color.Red, SymbolType.None);
            RPMGraph.GraphPane.XAxis.Scale.Max = fTime;
            RPMGraph.GraphPane.XAxis.Scale.Min = fTime - (8.0 * zoomFraction);
            RPMGraph.AxisChange();

            PressureGraph.GraphPane.AddCurve("", Datalist[0], Color.Red, SymbolType.None);
            PressureGraph.GraphPane.XAxis.Scale.Max = fTime;
            PressureGraph.GraphPane.XAxis.Scale.Min = fTime - (8.0 * zoomFraction);
            PressureGraph.AxisChange();
        }
        RectangleF a;
        public void InitializeGraph()
        {
            a = BreathGraph.GraphPane.Rect;
            InitializeGraph(BreathGraph);
            InitializeGraph(RPMGraph);
            InitializeGraph(PressureGraph);

            Datalist[(int)PacketDataType.eBreath] = new PointPairList();
            Datalist[(int)PacketDataType.eRPM] = new PointPairList();
            Datalist[(int)PacketDataType.ePressure] = new PointPairList();

        }
        public void InitializeGraph(ZedGraph.ZedGraphControl graph)
        {
            graph.MouseWheel += BreathGraph_MouseWheel;
            GraphPane myPane = graph.GraphPane;

            RectangleF tmp = myPane.Rect;
            tmp.X = a.X;
            myPane.ReSize(Graphics.FromHwnd(graph.Handle), tmp);

            graph.IsShowPointValues = true;           // Mouse over시 data 표시
            graph.PointValueFormat = "G";

            myPane.Fill.Color = System.Drawing.Color.WhiteSmoke;
            myPane.Legend.IsVisible = false;                                        // Legend
            myPane.Border.IsVisible = false;
            myPane.Title.IsVisible = false;

            myPane.XAxis.Scale.Format = "0.000";
            myPane.XAxis.Scale.MajorStep = 1.000;
            myPane.YAxis.Scale.Format = "0.0";

            //myPane.XAxis.Scale.MaxAuto = true;
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

            foreach(PointPairList list in Datalist)
                myPane.AddCurve("", list, Color.Red, SymbolType.None);

            graph.AxisChange();
            graph.Invalidate();

            myPane.YAxis.Scale.MaxAuto = true;
            myPane.YAxis.Scale.MinAuto = true;
        }
        public void test()
        {
            BreathGraph.MouseWheel += BreathGraph_MouseWheel;
            GraphPane myPane = BreathGraph.GraphPane;

            BreathGraph.IsShowPointValues = true;           // Mouse over시 data 표시
            BreathGraph.PointValueFormat = "G";

            myPane.Fill.Color = System.Drawing.Color.WhiteSmoke;
            myPane.Legend.IsVisible = false;                                        // Legend
            myPane.Border.IsVisible = false;
            myPane.Title.IsVisible = false;


            myPane.XAxis.Scale.Format = "0.000";
            myPane.XAxis.Scale.MajorStep = 1.000;
            myPane.YAxis.Scale.Format = "0.0";

            //myPane.XAxis.Scale.MaxAuto = true;
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


           // myPane.AddCurve("a", list, Color.Red, SymbolType.None);

            BreathGraph.AxisChange();
            BreathGraph.Invalidate();

            myPane.YAxis.Scale.MaxAuto = true;
            myPane.YAxis.Scale.MinAuto = true;

            //double[] x = new double[100];
            //double[] y = new double[100];
            //Random r = new Random();

            //int a = list.Count;
            // BreathGraph.BackColor = System.Drawing.Color.WhiteSmoke;                // Background Color
            // BreathGraph.BorderStyle = BorderStyle.None;
            //for (int i = 0; i < 100; i++)
            //{
            //    list.Add(((int)(i / 10) + i * 0.1), (r.NextDouble() * 10 - 5));
            //    //x[i] = (int)(i / 10) + i * 0.1;// +(r.NextDouble() / 100.0F);
            //    //y[i] = r.NextDouble() * 10 - 5;
            //}

            //myPane.Chart.Border = new Border();
            //myPane.PaneBorder = new Border(false, System.Drawing.Color.White, 0);   // Border
           // myPane.IsShowTitle = false;                                             // Title



            //myPane.XAxis.Type = AxisType.Ordinal;

            //XDate[] xd = new XDate[100];
            //for(int s = 0; s < 10; ++s)
            //{
            //    xd.AddSeconds(s);

            

            //myPane.XAxis.MajorTic = MajorTic


            
            //myPane.XAxis.ScaleFormat = "ss.SSS";
            ////myPane.XAxis.Scale.Format = "dd HH:mm:ss";


            //myPane.AddCurve("Sine Wave", x, y, Color.Red, SymbolType.Square);


            ////myPane.XAxis.Type = AxisType.
        }

    }
}
