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
    public static class Token       // 토큰을 정의해 놓은 클래스. c++의 #defin 역할.
    {
        public const char numToken = '\t';
        public const char seriesToken = '\n';
    }
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        public Form3(String str)
        {
            InitializeComponent();
            DrawGraph(str);
        }
        private void DrawGraph(String str)
        {
            int line = 0;
            int autoInc = 1;
            string num = "";
            foreach(char c in str){
                if (c == Token.seriesToken) {    // 라인 문자일 경우 line 변수를 올려서 시리즈의 인덱스 값 증가시킴.
                    line++;
                    autoInc = 1;
                }
                else {          //보통 문자일 경우 그래프에 값을 입력.
                    if (c != Token.numToken) {    // 탭 문자가 나올때 까지 숫자 string에 입력받음.
                        num += c;
                        continue;
                    }
                    graph.Series[line].Points.AddXY(autoInc, float.Parse(num)); //온전히 한 셀에 숫자를 다 입력받으면 그래프에 입력.
                    num = "";
                    autoInc++;
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
}
