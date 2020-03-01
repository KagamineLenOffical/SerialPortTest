using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;

namespace SerialPortTest
{
    public partial class Form1 : Form
    {
        SerialPort port;
        Thread thread;
        bool _continue;
        public Form1()
        {
            _continue = false;
            InitializeComponent();
        }
        public void CreateNewPort()
        {
            try
            {
                port = new SerialPort(comboBox1.SelectedItem.ToString(), Convert.ToInt32(BaudRate.Text), (Parity)Enum.Parse(typeof(Parity), comboBox2.Text, true),
                    Convert.ToInt32(TransBits.Text), (StopBits)Enum.Parse(typeof(StopBits), comboBox3.Text, true));
                port.ReadTimeout = 500;
                port.WriteTimeout = 500;  //无timeout会一直运行导致连接无法断开
                _continue = true;
                //textBoxAccept.AppendText("portLink:"+port.PortName + port.BaudRate.ToString() + port.Parity.ToString() + port.DataBits.ToString() + port.StopBits.ToString() + "\n");
                port.Open();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (!port.IsOpen)
            {
                textBoxAccept.AppendText("Cant open the port.\n");
                return;
            }
            thread = new Thread(ReceiveMessage);
            thread.IsBackground = true;
            thread.Start();
        }
        private void ReceiveMessage()
        {
            while (_continue)
            {
                try
                {
                    String s = port.ReadLine();
                    if (s != null)
                    {
                        string time = DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss.fff]");
                        s = time + "[Accept]" + s + "\n";
                        textBoxAccept.AppendText(s);
                    }
                }
                catch (TimeoutException) { }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (port!=null&&port.IsOpen)
            {
                textBoxAccept.AppendText("Port has been opened.\n");
                return;
            }
            CreateNewPort();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String s = textBoxSend.Text;
            port.WriteLine(s);
            string time = DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss.fff]");
            textBoxAccept.AppendText(time + "[Send]" + s+"\n");
            textBoxSend.Clear();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (port!=null&&port.IsOpen) port.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
            }
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                comboBox2.Items.Add(s);

            }
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                comboBox3.Items.Add(s);
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 1;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (port != null && port.IsOpen)
            {
                try
                {
                    _continue = false;
                    thread.Join();
                    Thread.Sleep(10); 
                    port.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
    
}
