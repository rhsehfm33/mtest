using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Termie
{
    public partial class Form3 : Form
    {
        private int autoInc = 1;
        public Form3()
        {
            InitializeComponent();
        }
        public Form3(String str)
        {
            InitializeComponent();
            DrawGraph(str);
        }
        public void DrawGraph(String str)
        {
            int series = 0;
            string num = "";        // 엑셀로부터 읽은 하나의 숫자 값(float). 
            foreach(char c in str){
                if (c == Token.seriesToken) {    // 라인 문자일 경우 line 변수를 올려서 시리즈의 인덱스 값 증가시킴.
                    graph.Series[series].Points.AddXY(autoInc, float.Parse(num)); //온전히 한 셀에 숫자를 다 입력받으면 그래프에 입력.
                    num = "";
                    series++;
                }
                else if(c == Token.lineToken)  {
                    series = 0;
                    autoInc++;
                }
                else {
                    num += c;
                }
            }
        }
        public void AddGraph(String str)
        {
            int series = 0;
            string num = "";        // 엑셀로부터 읽은 하나의 숫자 값(float). 
            foreach (char c in str)
            {
                if (c == Token.seriesToken)
                {    // 라인 문자일 경우 line 변수를 올려서 시리즈의 인덱스 값 증가시킴.
                    graph.Series[series].Points.AddXY(autoInc, float.Parse(num)); //온전히 한 셀에 숫자를 다 입력받으면 그래프에 입력.
                    num = "";
                    series++;
                }
                else if (c == Token.lineToken)
                {
                    series = 0;
                    autoInc++;
                }
                else
                {
                    num += c;
                }
            }
        }
        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
    public static class Token       // 토큰을 정의해 놓은 클래스. c++의 #defin 역할.
    {
        public const char seriesToken = '\t';
        public const char lineToken = '\n';
    }
}
