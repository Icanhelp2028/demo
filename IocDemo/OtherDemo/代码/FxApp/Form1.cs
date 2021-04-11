using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FxApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string fileName = "";
        const string dir = "history\\";
        const string ext = ".csv";
        private void ThreadRun()
        {
            List<Fx.Bar> bars = this.fxChart1.ChartView.Bars;

            string path = Path.GetFullPath(dir + fileName + ext);
            string[] lines = File.ReadAllLines(path);
            int i = 0;
            int n = lines.Length;
            string[] values = lines[0].Split(',');
            this.fxChart1.ChartView.Digits = (values[2].Length - values[2].IndexOf(".") - 1);
            while (i < n)
            {
                values = lines[i].Split(',');
                bars.Insert(0, new Fx.Bar
                {
                    Time = DateTime.Parse(values[0] + " " + values[1]),
                    Open = decimal.Parse(values[2]),
                    High = decimal.Parse(values[3]),
                    Low = decimal.Parse(values[4]),
                    Close = decimal.Parse(values[5]),
                    Volume = decimal.Parse(values[6])
                });
                this.fxChart1.Invalidate();
                i++;
                //Thread.Sleep(100 + (int)(100d * Math.Sin(i)));
                Thread.Sleep(100);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(dir, "*" + ext);
            if (files.Length > 0)
            {
                foreach (string file in files)
                    this.comboBox1.Items.Add(file.Substring(dir.Length, file.Length - dir.Length - ext.Length));
                this.comboBox1.SelectedIndex = 0;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.fxChart1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.fxChart1.ChartView.Zoom = (this.fxChart1.ChartView.Zoom) % 4 + 1;
            this.fxChart1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.fxChart1.ChartView.Style.Foreground = this.colorDialog1.Color;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.fxChart1.ChartView.Style.Barup = this.colorDialog1.Color;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.fxChart1.ChartView.Style.Bardown = this.colorDialog1.Color;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.fxChart1.ChartView.Style.Bullcandle = this.colorDialog1.Color;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.fxChart1.ChartView.Style.Bearcandle = this.colorDialog1.Color;
        }
        Thread thread = null;
        private void button7_Click(object sender, EventArgs e)
        {
            this.fxChart1.ChartView.Bars.Clear();
            fileName = this.comboBox1.SelectedItem.ToString();
            if (thread != null)
                thread.Abort();
            thread = new Thread(ThreadRun);
            thread.IsBackground = true;
            thread.Start();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}