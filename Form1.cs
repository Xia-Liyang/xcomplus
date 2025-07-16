using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xcomplus
{
    public partial class Form1 : Form
    {

        
        private Serial serial;
        private  Dictionary<string, object> sets = new Dictionary<string, object>();
        private SerialPort sp;
        Form2 form2 = new Form2();
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 1000; // 1秒  
            timer1.Tick += timer1_Tick; // 添加事件  
            timer1.Start(); // 开始
            
            object[] baud = new object[] { 9600, 115200 };
            comboBox2.Items.AddRange(baud);
            object[] data = new object[] { 8 };
            comboBox3.Items.AddRange(data);
            object[] stop = new object[] { 1 };
            comboBox4.Items.AddRange(stop);
            object[] right = new object[] { "None" };
            comboBox5.Items.AddRange(right);
            serial = new Serial(comboBox1);
            sets["port"] = comboBox1;
            sets["baud"] = comboBox2;
            sets["data"] = comboBox3;
            sets["stop"] = comboBox4;
            sets["right"] = comboBox5;
            sets["openSerial"] = checkBox1;


        }

        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serial.ReceiveData(sp);

            // 在主线程中更新 UI 和写数据库
            this.Invoke(new Action(() =>
            {
                if (!checkBox3.Checked)
                {
                    textBox1.AppendText(DateTime.Now.ToString("F") + ":" +"  "+ data + Environment.NewLine);
                    // ✅ 插入数据库
                    sqlserver db = new sqlserver(new DataTable()); // 或用 static 实例，避免每次 new
                    db.InsertToDatabase(DateTime.Now.ToString("F") + ":" + "  " + data + Environment.NewLine);
                    //form2.Refresh();
                }
                else
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(data);
                    string hexString = BitConverter.ToString(bytes).Replace("-", " ");
                    textBox1.AppendText(hexString + Environment.NewLine);
                }


            }));

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (comboBox1.SelectedIndex == -1)
                {

                    MessageBox.Show("错误,选择串口！");
                    checkBox1.Checked = false;
                }
                else
                {
                    try
                    {
                        sp = serial.CreateSerialPort(sets);
                        sp.Open(); // 确保串口打开成功
                        sp.DataReceived += Sp_DataReceived; // 绑定事件
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("串口打开失败: " + ex.Message);
                        checkBox1.Checked = false;
                    }
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //sets["port"] = comboBox1;
            //serial.updata_Set(sets, sp);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }


        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            string timeOnly = DateTime.Now.ToString("F");  // 比如：14:35:20  
            label8.Text = "TIME:" + "    " + timeOnly;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (form2.Visible)
            {
                form2.Hide();
            }
            else
            {
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!form2.Visible&& Application.OpenForms["Form2"] != null)
            {
                form2.Show();
            }
            else if(Application.OpenForms["Form2"] == null)
            {
                form2 = new Form2();
                form2.Show();
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

            //sets["stop"] = comboBox4;
            //serial.updata_Set(sets, sp);
          
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //sets["baud"] = comboBox2;
            //serial.updata_Set(sets, sp);
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            /////////////////////////////////////
            serial.find_serial(comboBox1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

            //sets["data"] = comboBox3;
            //serial.updata_Set(sets, sp);

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

            //sets["right"] = comboBox5;
            //serial.updata_Set(sets, sp);

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
        }
    }
}
