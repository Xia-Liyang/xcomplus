using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;


namespace xcomplus
{
    internal class Serial
    {
        /// <summary>
        /// 
        /// 这里是构造函数，检测串口，并通过查找找到串口所对应的名称
        /// </summary>
        /// <param name="comboBox"></param>
        /// 这里需要传入作为串口名的comobox
        /// 
        public Serial(System.Windows.Forms.ComboBox comboBox) 
        {
            find_serial(comboBox);
        }

        public void find_serial(System.Windows.Forms.ComboBox comboBox)
        {
            comboBox.Items.Clear();
            int index = 1;
            //检测有效的串口并添加到combobox
            //string[]  serialPortName_temp = System.IO.Ports.SerialPort.GetPortNames();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PnPEntity");
            var hardInfos = searcher.Get();
            foreach (var name in hardInfos)
            {
                if (name.Properties["Name"].Value != null && name.Properties["Name"].Value.ToString().Contains("(COM"))
                {
                    String strComName = name.Properties["Name"].Value.ToString();
                    comboBox.Items.Add(index + ":" + strComName);
                    index += 1;
                }

            }
        }

        /// <summary>
        /// 用于创建一个新的通信串口
        /// </summary>
        /// <param name="sets"></param>
        /// 该参数为一个字典，其中键值对分别为：port，baud，data，stop，right所对应的comobox
        /// 在函数内部会解析其选择的值，并转化为新建串口所需要的参数值
        /// <returns></returns>
        /// 返回值为一个串口对象
        public SerialPort CreateSerialPort(Dictionary<string,object> sets)
        {
            // 获取各个 ComboBox
            ComboBox combox1 = sets["port"] as ComboBox;
            ComboBox combox2 = sets["baud"] as ComboBox;
            ComboBox combox3 = sets["data"] as ComboBox;
            ComboBox combox4 = sets["stop"] as ComboBox;
            ComboBox combox5 = sets["right"] as ComboBox;

            SerialPort sp = new SerialPort();

            // 1. 端口号（COM3）
            if (combox1?.SelectedItem != null)
            {
                string selectedText = combox1.SelectedItem.ToString();
                int start = selectedText.IndexOf("COM");
                if (start >= 0)
                {
                    int end = selectedText.IndexOf(")", start);
                    if (end > start)
                    {
                        string portName = selectedText.Substring(start, end - start);
                        sp.PortName = portName;
                    }
                }
            }

            // 2. 波特率
            if (combox2?.SelectedItem != null)
            {
                sp.BaudRate = int.Parse(combox2.SelectedItem.ToString());
            }

            // 3. 数据位
            if (combox3?.SelectedItem != null)
            {
                sp.DataBits = int.Parse(combox3.SelectedItem.ToString());
            }

            // 4. 停止位
            if (combox4?.SelectedItem != null)
            {
                switch (combox4.SelectedItem.ToString())
                {
                    case "1": sp.StopBits = StopBits.One; break;
                    case "1.5": sp.StopBits = StopBits.OnePointFive; break;
                    case "2": sp.StopBits = StopBits.Two; break;
                }
            }

            // 5. 校验位
            if (combox5?.SelectedItem != null)
            {
                switch (combox5.SelectedItem.ToString())
                {
                    case "None": sp.Parity = Parity.None; break;
                    case "Odd": sp.Parity = Parity.Odd; break;
                    case "Even": sp.Parity = Parity.Even; break;
                    case "Mark": sp.Parity = Parity.Mark; break;
                    case "Space": sp.Parity = Parity.Space; break;
                }
            }

            return sp;
        }


        /// <summary>
        /// 更新串口设置
        /// </summary>
        /// <param name="sets"></param>
        /// 参数为一个字典，其中键值对分别为：port，baud，data，stop，right所对应的comobox
        /// <param name="sp"></param>
        /// 需要更新的串口对象
        /// <returns></returns>
        /// 返回更新后的串口对象
        public SerialPort updata_Set(Dictionary<string, object> sets, SerialPort sp)
        {
            // 获取各个 ComboBox
            ComboBox combox1 = sets["port"] as ComboBox;
            ComboBox combox2 = sets["baud"] as ComboBox;
            ComboBox combox3 = sets["data"] as ComboBox;
            ComboBox combox4 = sets["stop"] as ComboBox;
            ComboBox combox5 = sets["right"] as ComboBox;

            if (sp != null && sp.IsOpen)
            {
                sp.Close();
            }

            else
            {
                
                // 更新串口设置
                if (combox1?.SelectedItem != null)
                {
                    string selectedText = combox1.SelectedItem.ToString();
                    int start = selectedText.IndexOf("COM");
                    if (start >= 0)
                    {
                        int end = selectedText.IndexOf(")", start);
                        if (end > start)
                        {
                            string portName = selectedText.Substring(start, end - start);
                            sp.PortName = portName;
                        }
                    }
                }

                if (combox2?.SelectedItem != null)
                {
                    sp.BaudRate = int.Parse(combox2.SelectedItem.ToString());
                }

                if (combox3?.SelectedItem != null)
                {
                    sp.DataBits = int.Parse(combox3.SelectedItem.ToString());
                }

                if (combox4?.SelectedItem != null)
                {
                    switch (combox4.SelectedItem.ToString())
                    {
                        case "1": sp.StopBits = StopBits.One; break;
                        case "1.5": sp.StopBits = StopBits.OnePointFive; break;
                        case "2": sp.StopBits = StopBits.Two; break;
                    }
                }

                if (combox5?.SelectedItem != null)
                {
                    switch (combox5.SelectedItem.ToString())
                    {
                        case "None": sp.Parity = Parity.None; break;
                        case "Odd": sp.Parity = Parity.Odd; break;
                        case "Even": sp.Parity = Parity.Even; break;
                        case "Mark": sp.Parity = Parity.Mark; break;
                        case "Space": sp.Parity = Parity.Space; break;
                    }
                }
                sp.Open();
            }
            
            return sp;
        }
        public string ReceiveData(SerialPort sp)
        {
            string data;
            if (sp == null) return null;
            else
            {
                if (!sp.IsOpen) return null;
                else
                {
                  data =  sp.ReadExisting();
                }
            }
            return data;
        }

    }
}
