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
using System.Windows.Forms.DataVisualization.Charting;
using System.Runtime.Serialization;

namespace bai3.Auto_detectcom
{
    public partial class Form1 : Form
    {

       

        public Form1()
        {
            InitializeComponent();
            
        }
        int t = 0;
        double setpoint;
        float temp_sp;
        double tocdo;
        int byte_count;
        int ack=0;
        byte[] data;
        List<double> yr = new List<double>();
        List<double> sp = new List<double>();
        int tongsocom = 0;
        delegate void SetTextCallBack(String Text);//khai bao thu tuc delegate
        byte[] package = new byte[24];

        UInt16 Sin_mode = 0, Sin_start = 0, Tri_mode = 0, Tri_start = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Press OK to continue,then choose port and press button Connect", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void CBSecCom_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames(); // lay ra tat ca cac cong port
            if(tongsocom != ports.Length)
            {
                tongsocom = ports.Length;
                CBSecCom.Items.Clear();
                for(int j=0;j<tongsocom;j++)
                {
                    CBSecCom.Items.Add(ports[j]);
                }

            }
        }
          private void pbconnect_Click(object sender, EventArgs e)
        {
            if(LBsatus.Text == "Disconnect")
            {
                com.PortName = CBSecCom.Text;
                com.Open();
                LBsatus.Text = "Connect";
                pbconnect.Text = "Disconnect";
                PbST_TIMER2.Text = "Start";
                CbSin.Enabled = true;
                CbTri.Enabled = true;
                txtA1.Enabled = true;
                txtA2.Enabled = true;
                txtf1.Enabled = true;
                PbSend.Enabled = true;
                txtf2.Enabled = true;
                PbST_TIMER2.Enabled = true;
                CbSinMode1.Enabled = true;
                CbSinMode2.Enabled = true;
                CbTriMode1.Enabled = true;
                CbTriMode2.Enabled = true;

            }
            else
            {
                com.Close();
                LBsatus.Text = "Disconnect";
                pbconnect.Text = "Connect";
                CbSin.Enabled = false;
                CbTri.Enabled = false;
                txtA1.Enabled = false;
                txtA2.Enabled = false;
                txtf1.Enabled = false;
                PbSend.Enabled = false;
                txtf2.Enabled = false;
                CbSinMode1.Enabled = false;
                CbSinMode2.Enabled = false;
                CbTriMode1.Enabled = false;
                CbTriMode2.Enabled = false;
                CbSin.Checked = false;
                CbTri.Checked = false;
                timer1.Enabled = false;
                timer2.Enabled = false;
                PbST_TIMER2.Enabled = false;
                MessageBox.Show("Serial port is closed", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // send data

        private void PbSend_Click(object sender, EventArgs e)
        {
           
            if (LBsatus.Text == "Connect")
            {
                package[0] = Convert.ToByte('d');
                package[1] = Convert.ToByte('k');
                package[2] = Convert.ToByte('n');
                package[3] = Convert.ToByte('h');
                if (Sin_start == 1)  package[4] = Convert.ToByte('r'); 
                else package[4] = Convert.ToByte('s');
                if (Sin_mode == 1) package[5] = Convert.ToByte('b');
                else package[5] = Convert.ToByte('t');
                if (Tri_start == 1) package[6] = Convert.ToByte('r');
                else package[6] = Convert.ToByte('s');
                if (Tri_mode == 1) package[7] = Convert.ToByte('b');
                else package[7] = Convert.ToByte('t');
                byte[] temp;
                temp = System.BitConverter.GetBytes(Convert.ToSingle(txtA1.Text));
                for (int c = 8; c <= 11; c++)
                {
                    package[c] = temp[c - 8];
                }
                temp = System.BitConverter.GetBytes(Convert.ToSingle(txtf1.Text));
                for (int c = 12; c <= 15; c++)
                {
                    package[c] = temp[c - 12];
                }
                temp = System.BitConverter.GetBytes(Convert.ToSingle(txtA2.Text));
                for (int c = 16; c <= 19; c++)
                {
                    package[c] = temp[c - 16];
                }
                temp = System.BitConverter.GetBytes(Convert.ToSingle(txtf2.Text));
                for (int c = 20; c <= 23; c++)
                {
                    package[c] = temp[c - 20];
                }
                com.Write(package, 0, 24);

            }
           

        }

         

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pbExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void txtReceive_TextChanged(object sender, EventArgs e)
        {
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (LBsatus.Text == "Connect")
            {

                yr.Add(tocdo);


                sp.Add(setpoint);
                t = t + 1;


            }

            drawchart();
        }
        public void drawchart()
        {
            int init_tmp;
            this.chart1.Series.Clear();
            Series response = this.chart1.Series.Add("Speed");
            Series reference = this.chart1.Series.Add("Setpoint");
            reference.ChartType = SeriesChartType.Line;
            response.ChartType = SeriesChartType.Line;
            reference.Color = Color.Red;
            double[] yr_val = yr.ToArray();
            double[] sp_val = sp.ToArray();
            if (t > 500) init_tmp = t - 500;
            else init_tmp = 0;
            for (int i = init_tmp; i < t; i++)
            {
               response.Points.AddXY(0.001 * i, yr_val[i]);
               reference.Points.AddXY(0.001 * i,sp_val[i]);
               

            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void pbquaythuan_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void Oncom(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (com.IsOpen)
                {
                    byte_count = com.BytesToRead;
                    data = new byte[byte_count];
                    com.Read(data, 0, byte_count);
                    if (data[0] == 'd' && data[1] == 'k' && data[2] == 'm' && data[3] == 't' && data[4] == 'r') ack = 0;
                    else if (data[0] == 'd' && data[1] == 'k' && data[2] == 'm' && data[3] == 't' && data[4] == 'v') 
                    { 
                        tocdo= System.BitConverter.ToSingle(data, 8);
                    }
                }
            }
            catch
            {

            }

            //string s;
            //s = com.ReadExisting();
            //SetTextFeedBack(s);
            //double.TryParse(s, out tocdo);
            //drawchart();


        }
        private void SetTextFeedBack(string text)
        {
            
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void Timer3_Tick(object sender, EventArgs e)
        {

           /// 
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void pbquaynghich_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void txtA1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtf1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtA2_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtf2_TextChanged(object sender, EventArgs e)
        {

        }

        private void CbSin_CheckedChanged_1(object sender, EventArgs e)
        {
            if (CbSin.Checked == true)
            {
                Sin_start = 1;
            }
            else
            {
                Sin_start = 0;
            }
        }

        private void CbTri_CheckedChanged(object sender, EventArgs e)
        {
            if (CbTri.Checked == true)
            {
                Tri_start = 1;
            }
            else
            {
                Tri_start = 0;
            }
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void CbTriMode2_CheckChange(object sender, EventArgs e)
        {
            if (CbTriMode2.Checked == true)
            {
                CbTriMode1.Checked = false;
                Tri_mode = 1;
            }
        }

        private void CbTriMode1_CheckChange(object sender, EventArgs e)
        {
            if (CbTriMode1.Checked == true)
            {
                CbTriMode2.Checked = false;
                Tri_mode = 0;
            }
        }

        private void CbSinMode1_CheckChange(object sender, EventArgs e)
        {
            if (CbSinMode1.Checked == true)
            {
                CbSinMode2.Checked = false;
                Sin_mode = 0;
            }
        }

        private void CbSinMode2_CheckChange(object sender, EventArgs e)
        {
            if (CbSinMode2.Checked == true)
            {
                CbSinMode1.Checked = false;
                Sin_mode = 1;
            }
        }

        private void PbST_TIMER2_Click(object sender, EventArgs e)
        {
            if (LBsatus.Text == "Connect")
            {
                if (PbST_TIMER2.Text == "Stop")
                {
                    PbST_TIMER2.Text = "Start";
                    timer2.Enabled = false;
                    //PbSend.Enabled = false;
                    //setpoint_ = "000";

                }
                else
                {
                     PbST_TIMER2.Text = "Stop";
                     timer2.Enabled = true;
                    //PbSend.Enabled = true;

                }
            }
        }

    }
}
