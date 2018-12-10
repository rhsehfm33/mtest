using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;

namespace Termie
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();

            CommPort com = CommPort.Instance;

            int found = 0;
            string[] portList = com.GetAvailablePorts();
            for (int i = 0; i < portList.Length; ++i)
            {
                string name = portList[i];
                comboBox1.Items.Add(name);
                if (name == Settings.Port.PortName)
                    found = i;
            }
            if (portList.Length > 0)
                comboBox1.SelectedIndex = found;

            Int32[] baudRates = {
                100,300,600,1200,2400,4800,9600,14400,19200,
                38400,56000,57600,115200,128000,256000,0
            };
            found = 0;
            for (int i = 0; baudRates[i] != 0; ++i)
            {
                comboBox2.Items.Add(baudRates[i].ToString());
                if (baudRates[i] == Settings.Port.BaudRate)
                    found = i;
            }
            comboBox2.SelectedIndex = found;

            comboBox3.Items.Add("5");
            comboBox3.Items.Add("6");
            comboBox3.Items.Add("7");
            comboBox3.Items.Add("8");
            comboBox3.SelectedIndex = Settings.Port.DataBits - 5;

            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                comboBox4.Items.Add(s);
            }
            comboBox4.SelectedIndex = (int)Settings.Port.Parity;

            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                comboBox5.Items.Add(s);
            }
            comboBox5.SelectedIndex = (int)Settings.Port.StopBits;

            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                comboBox6.Items.Add(s);
            }
            comboBox6.SelectedIndex = (int)Settings.Port.Handshake;

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Settings.Port.PortName = comboBox1.Text;
            Settings.Port.BaudRate = Int32.Parse(comboBox2.Text);
            Settings.Port.DataBits = comboBox3.SelectedIndex + 5;
            Settings.Port.Parity = (Parity)comboBox4.SelectedIndex;
            Settings.Port.StopBits = (StopBits)comboBox5.SelectedIndex;
            Settings.Port.Handshake = (Handshake)comboBox6.SelectedIndex;

            CommPort com = CommPort.Instance;
            com.Open();

            Settings.Write();

            Close();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Close();
        }

    }
}
