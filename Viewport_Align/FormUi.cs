using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace RAD_Iot
{
    public partial class FormUi : Form
    {
        String[] ports;
        SerialPort port;
        List<int> intList = new List<int>();
        public FormUi()
        {
            InitializeComponent();
            ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
                Console.WriteLine(port);
                if (ports[0] != null)
                {
                    comboBox1.SelectedItem = ports[0];
                }
            }
        }

        private void GetPort(object sender, EventArgs e)
        {
            string selectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);
            port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
           // port.Open();
        }

        private string WriteToPort()
        {
            string xSteps = tb_X.Text;
            string ySteps = tb_Y.Text;
            string zSteps = tb_Z.Text;
            string feedRate = tb_F.Text;

            string dataArray = xSteps.ToString() + "," + ySteps.ToString() + "," + zSteps.ToString() + "," + feedRate.ToString() + Environment.NewLine;

            port.Open();
            if (port.IsOpen)
            {
                port.Write(dataArray);
            }
            port.Close();

            return dataArray;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string xSteps = tb_X.Text;
            string ySteps = tb_Y.Text;
            string zSteps = tb_Z.Text;
            string feedRate = tb_F.Text;

            string dataArray = xSteps.ToString() + "," + ySteps.ToString() + "," + zSteps.ToString() + "," + feedRate.ToString() + Environment.NewLine;

            port.Open();
            if (port.IsOpen)
            {
                port.Write(dataArray);
            }
            port.Close();

            label5.Text = dataArray;
            //MessageBox.Show("Form for interface");
        }
    }
}
