using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Windows.Forms.DataVisualization.Charting;

namespace SerialPlotter
{
    public partial class Form1 : Form
    {

        int MAX_HISTORY = 30;
        int AVERAGE = 1;
        
        Queue<double> Qdata1 = new Queue<double>();
        Queue<double> Qdata2 = new Queue<double>();
        Queue<double> Qdata3 = new Queue<double>();
        Queue<double> Qdata4 = new Queue<double>();
        string filename;
        string exePath;

        int count;
        double sum1, sum2, sum3, sum4;
        
        

        private class BuadItem : Object
        {
            private string name = "";
            private int val = 0;
            
            public string NAME
            {
                set { name = value; }
                get { return name; }
            }    
        
            public int BAUDRATE
            {
                set { val = value; }
                get { return val; }
            }
           
            public override string ToString()
            {
                return name;
            }
        }

        private void showChart(Chart chart, Queue<int>  Qdata)
        {

            chart.Series[0].Points.Clear();
            foreach (int value in Qdata)
            {
                chart.Series[0].Points.Add(new DataPoint(0, value));
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void BTN_CON_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {

                try
                {
                    System.Windows.Forms.Application.DoEvents();
                    if (true == serialPort1.IsOpen) serialPort1.DiscardInBuffer();
                    System.Windows.Forms.Application.DoEvents();
                    if (true == serialPort1.IsOpen) serialPort1.Close();
                    CMB_BAUD.Enabled = true;
                    CMB_COM.Enabled = true;
                    System.Windows.Forms.Application.DoEvents();
                    count = 1;



                    BeginInvoke(new Delegate_ChangeButton(ChangeButton), new Object[] { "Connect" });

                   
                   
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }

            }
            else {

                serialPort1.PortName = CMB_COM.SelectedItem.ToString();
				
                BuadItem baud = (BuadItem)CMB_BAUD.SelectedItem;
				serialPort1.BaudRate = baud.BAUDRATE;				
				serialPort1.DataBits = 8;				
				serialPort1.Parity = Parity.None;				
				serialPort1.StopBits = StopBits.One;	
                serialPort1.Handshake = Handshake.None;				
				serialPort1.Encoding = Encoding.ASCII;
                CMB_BAUD.Enabled = false;
                CMB_COM.Enabled = false;
                count = 1;

                /*Get File Name*/
                exePath = System.IO.Directory.GetCurrentDirectory();
                DateTime dt = DateTime.Now;
                filename = dt.Year.ToString("0000") + dt.Month.ToString("00") + dt.Day.ToString("00")
                                  + dt.Hour.ToString("00") + dt.Minute.ToString("00") + dt.Second.ToString("00")+".csv";

                try
                {
                    serialPort1.Open();
                    if (true == serialPort1.IsOpen) serialPort1.DiscardInBuffer();
                    BeginInvoke(new Delegate_ChangeButton(ChangeButton), new Object[] { "Disconnect" });
                  

                    
                   
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }

        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
          


            BeginInvoke(new Delegate_GetDataText(GetDataText), new Object[] { CMB_DATA });
            BeginInvoke(new Delegate_GetAveText(GetAveText), new Object[] { CMB_AVE });


            string[] StrArryData = new string[10];


            if( serialPort1.IsOpen == false )	return;


            try {
                string comdata =  serialPort1.ReadLine();//ReadExisting()                
                StrArryData = comdata.Split(new string[] { "," }, StringSplitOptions.None);

                if (checkBox1.Checked)
                {
                             


                    string csvPath = exePath + "\\" + filename;
                    System.Text.Encoding enc =
                        System.Text.Encoding.GetEncoding("Shift_JIS");
                    System.IO.StreamWriter sr =
                        new System.IO.StreamWriter(csvPath, true, enc);
                    sr.WriteLine(comdata);
                    sr.Close();


                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }

           

           


            double tmp0=0.0, tmp1=0.0, tmp2=0.0, tmp3=0.0;
            if (StrArryData.Length > 0) double.TryParse(StrArryData[0], out tmp0);
            if (StrArryData.Length > 1) double.TryParse(StrArryData[1], out tmp1);
            if (StrArryData.Length > 2) double.TryParse(StrArryData[2], out tmp2);
            if (StrArryData.Length > 3) double.TryParse(StrArryData[3], out tmp3);

            
            
            if (count < AVERAGE)
            {
                sum1 = sum1 + tmp0;
                sum2 = sum2 + tmp1;
                sum3 = sum3 + tmp2;
                sum4 = sum4 + tmp3;

                count++;
            }
            else {
                
                sum1 = sum1 + tmp0;
                sum2 = sum2 + tmp1;
                sum3 = sum3 + tmp2;
                sum4 = sum4 + tmp3;

               
                Qdata1.Enqueue(sum1 / (double)AVERAGE);
                Qdata2.Enqueue(sum2 / (double)AVERAGE);
                Qdata3.Enqueue(sum3 / (double)AVERAGE);
                Qdata4.Enqueue(sum4 / (double)AVERAGE);
                sum1 = 0;
                sum2 = 0;
                sum3 = 0;
                sum4 = 0;
                count = 1;


            }


            while (Qdata1.Count > MAX_HISTORY) Qdata1.Dequeue();
            while (Qdata2.Count > MAX_HISTORY) Qdata2.Dequeue();
            while (Qdata3.Count > MAX_HISTORY) Qdata3.Dequeue();
            while (Qdata4.Count > MAX_HISTORY) Qdata4.Dequeue();

            BeginInvoke(new Delegate_ShowChart(ShowChart1), new Object[] { chart1 });
            BeginInvoke(new Delegate_ShowChart(ShowChart2), new Object[] { chart2 });
            BeginInvoke(new Delegate_ShowChart(ShowChart3), new Object[] { chart3 });
            BeginInvoke(new Delegate_ShowChart(ShowChart4), new Object[] { chart4 });


			
        }


        private delegate void Delegate_ShowChart(Chart chart);
        private delegate void Delegate_GetDataText(Control CON);
        private delegate void Delegate_GetAveText(Control CON);
        private delegate void Delegate_ChangeButton(string text);


        private void ShowChart1(Chart chart) {

            
            chart.Series[0].Points.Clear();
            chart.Series[0].MarkerStyle = MarkerStyle.Circle;
            chart.Series[0].MarkerSize = 6;
            chart.Series[0].Color=Color.Blue;

            if (Qdata1.Count > 2) {
                chart.ChartAreas[0].AxisY.Minimum = (int)Qdata1.Min();
               
                
            }
           
            foreach (int value in Qdata1)chart.Series[0].Points.Add(new DataPoint(0, value));
        }





        private void ShowChart2(Chart chart)
        {


            chart.Series[0].Points.Clear();
            chart.Series[0].MarkerStyle = MarkerStyle.Circle;
            chart.Series[0].MarkerSize = 6;
            chart.Series[0].Color = Color.Green;

            if (Qdata2.Count > 2)
            {
                chart.ChartAreas[0].AxisY.Minimum = (int)Qdata2.Min();
             
                
            }

            foreach (int value in Qdata2) chart.Series[0].Points.Add(new DataPoint(0, value));
        }
        private void ShowChart3(Chart chart)
        {


            chart.Series[0].Points.Clear();
            chart.Series[0].MarkerStyle = MarkerStyle.Circle;
            chart.Series[0].MarkerSize = 6;
            chart.Series[0].Color = Color.Red;

            if (Qdata3.Count > 2)
            {
                chart.ChartAreas[0].AxisY.Minimum = (int)Qdata3.Min();
               
                 
            }

            foreach (int value in Qdata3) chart.Series[0].Points.Add(new DataPoint(0, value));
        }
        private void ShowChart4(Chart chart)
        {


            chart.Series[0].Points.Clear();
            chart.Series[0].MarkerStyle = MarkerStyle.Circle;
            chart.Series[0].MarkerSize = 6;
            chart.Series[0].Color = Color.Violet;

            if (Qdata4.Count > 2)
            {
                chart.ChartAreas[0].AxisY.Minimum = (int)Qdata4.Min();
               
                
            }

            foreach (int value in Qdata4) chart.Series[0].Points.Add(new DataPoint(0, value));
        }




        private void GetDataText(Control CON)
        {
            int.TryParse(CON.Text, out MAX_HISTORY);
            
        }

        private void GetAveText(Control CON)
        {

            int.TryParse(CON.Text, out AVERAGE);

        }

        private void ChangeButton(string text) {

            BTN_CON.Text = text;
        
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Init_Control();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (true == serialPort1.IsOpen) serialPort1.DiscardInBuffer();
                System.Windows.Forms.Application.DoEvents();
                if (serialPort1.IsOpen) serialPort1.Close();
                System.Windows.Forms.Application.DoEvents();
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message);
            }
        }


        private void Init_Control(){

            /*Set COM Port List*/
            string[] ports = SerialPort.GetPortNames();
            CMB_COM.Items.Clear();
            foreach (string prtname in ports)
            {
                CMB_COM.Items.Add(prtname);
            }
            if (CMB_COM.Items.Count > 0)
            {
                CMB_COM.SelectedIndex = CMB_COM.Items.Count - 1;
            }

            /*Set Baud Rate*/
            BuadItem baud;
            baud = new BuadItem();
            baud.NAME = "4800bps";
            baud.BAUDRATE = 4800;
            CMB_BAUD.Items.Add(baud);

            baud = new BuadItem();
            baud.NAME = "9600bps";
            baud.BAUDRATE = 9600;
            CMB_BAUD.Items.Add(baud);

            baud = new BuadItem();
            baud.NAME = "19200bps";
            baud.BAUDRATE = 19200;
            CMB_BAUD.Items.Add(baud);

            baud = new BuadItem();
            baud.NAME = "115200bps";
            baud.BAUDRATE = 115200;
            CMB_BAUD.Items.Add(baud);
            CMB_BAUD.SelectedIndex = 1;


            /*Set Average List*/
            CMB_AVE.Items.Add(1);
            for (int i = 1; i < 11; i++)
            {
                CMB_AVE.Items.Add(2 * i);
            }
            CMB_AVE.SelectedIndex = 0;

            /*Set Data List*/
            CMB_DATA.Items.Add(1);
            for (int i = 1; i < 11; i++)
            {
                CMB_DATA.Items.Add(i * 30);
            }
            CMB_DATA.SelectedIndex = 2;
        }

        public static int CountChar(string s, char c)
        {
            return s.Length - s.Replace(c.ToString(), "").Length;
        }
}
}
