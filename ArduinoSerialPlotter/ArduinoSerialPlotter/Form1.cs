using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace ArduinoSerialPlotter
{
    public partial class Form1 : Form
    {
        string RxString;

        public Form1()
        {
            InitializeComponent();
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            RxString = serialPort1.ReadLine(); //シリアル通信の入力
            this.Invoke(new EventHandler(DisplayText));
            this.Invoke(new EventHandler(showChart));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true) //すでにポートが開かれている場合
            {
                MessageBox.Show("COMポートを閉じてください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox1.Text != "COMポート" && comboBox2.Text != "転送速度") //comboBox1とcomboBox2に設定があればシリアルポートを開く
            {
                serialPort1.PortName = comboBox1.Text; //comboBox1.Textをシリアルポートのポート名として設定

                serialPort1.BaudRate = int.Parse(comboBox2.Text);

                if (serialPort1.IsOpen == true) //シリアルポートが使われていたら一旦閉じる
                {
                    serialPort1.Close();
                }

                if (serialPort1.IsOpen == false) //シリアルポートが使われていないのを確認して開く
                {
                    serialPort1.Open();
                }

                if (serialPort1.IsOpen == true)
                {
                    MessageBox.Show("接続成功\n" + serialPort1.PortName.ToString(), "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("COMポートエラー", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (comboBox1.Text == "COMポート" && comboBox2.Text != "転送速度") //COMポートが選択されていない場合
            {
                MessageBox.Show("COMポートを選択してください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (comboBox1.Text != "COMポート" && comboBox2.Text == "転送速度") //転送速度が選択されていない場合
            {
                MessageBox.Show("転送速度を選択してください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else //どちらも選択されていない場合
            {
                MessageBox.Show("COMポートと転送速度を選択してください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close(); //切断
            comboBox1.Text = "COMポート";
            comboBox2.Text = "転送速度";
            MessageBox.Show("切断されました", "切断", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DisplayText(object sender, EventArgs e)
        {
            textBox1.AppendText(RxString + Environment.NewLine); //テキストボックスに出力
        }

        private void showChart(object sender, EventArgs e)
        {
            if (RxString.Contains(",")) //","が含まれていない場合に例外がスローされるのを防ぐ
            {
                string[] strArrayData = RxString.Split(',');
                int x = int.Parse(strArrayData[0]);
                int y = int.Parse(strArrayData[1]);

                chart1.Series["気温"].Points.AddXY(x, y);

                chart1.ChartAreas[0].AxisX.Maximum = 3600;
                chart1.ChartAreas[0].AxisX.Minimum = 0;
                if (x > 3600)
                {
                    chart1.ChartAreas[0].AxisX.Maximum = x;
                    chart1.ChartAreas[0].AxisX.Minimum = x - 3600;
                }

                chart1.ChartAreas[0].AxisY.Maximum = 40;
                chart1.ChartAreas[0].AxisY.Minimum = 10;
            }
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();

            comboBox1.Items.Clear();

            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
        }
    }
}
