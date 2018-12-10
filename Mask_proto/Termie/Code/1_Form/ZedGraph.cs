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
        public void test()
        {
            GraphPane myPane = BreathGraph.GraphPane;

            BreathGraph.IsShowPointValues = true;           // Mouse over시 data 표시
            BreathGraph.PointValueFormat = "G";

            myPane.PaneFill = new Fill(System.Drawing.Color.WhiteSmoke);            // Background Color
            myPane.PaneBorder = new Border(false, System.Drawing.Color.White, 0);   // Border
            myPane.Legend.IsVisible = false;                                        // Legend
            myPane.IsShowTitle = false;                                             // Title

            double[] x = new double[100];
            double[] y = new double[100];
            Random r = new Random();

            for (int i = 0; i < 100; i++)
            {
                x[i] = (i / 10) + (r.NextDouble() / 100.0F);
                y[i] = r.NextDouble() * 10 - 5;
            }

           myPane.XAxis.Type = AxisType.Ordinal;

            myPane.AddCurve("Sine Wave", x, y, Color.Red, SymbolType.Square);
            BreathGraph.AxisChange();
            BreathGraph.Invalidate();
        }
    }
}
