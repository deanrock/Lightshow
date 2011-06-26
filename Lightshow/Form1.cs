using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.IO;
using CoreAudioApi;

namespace Lightshow
{
    public partial class Form1 : Form
    {
        private static System.IO.Ports.SerialPort serialPort1;
        private MMDevice device;
        static int _bits;
        static int _device_bits;
        int tbMaster;
        private string previousStatusString = "";

        private int CEILING_LIGHT = 7;

        //knight rider
        private int knightrider = 0;
        private int knightriderDirection = 0;

        private string mode = "music01110";

        private string httpRequest(string url)
        {
            return "";
            WebRequest request = WebRequest.Create (url);
 
            // For HTTP, cast the request to HttpWebRequest
            // allowing setting more properties, e.g. User-Agent.
            // An HTTP response can be cast to HttpWebResponse.
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    // Ensure that the correct encoding is used. 
                    // Check the response for the Web server encoding.
                    // For binary content, use a stream directly rather
                    // than wrapping it with StreamReader.

                    using (StreamReader reader = new StreamReader
                       (response.GetResponseStream(), Encoding.UTF8))
                    {
                        string content = reader.ReadToEnd();
                        // process the content
                        return content;
                    }

                }
            }
            catch (Exception) { }

            return "";
        }

          
        

        static void switchIt(int index, bool x)
        {
            

            if (x)
            {
                _bits |= (1 << index);  //set bit index 1
            }
            else
            {
                _bits &= ~(1 << index); //set bit index 0  
            }
        }

        static void switchItDevice(int index, bool x)
        {


            if (x)
            {
                _device_bits |= (1 << index);  //set bit index 1
            }
            else
            {
                _device_bits &= ~(1 << index); //set bit index 0  
            }
        }

        void s(int x1, int x2, int x3, int x4, int x5, int x6, int x7, int x8, int x9, int x10)
        {
            sw2(x6, x7, x8, x9, x10, 0);
            sw2(x5, x4, x3, x2, x1, 1);

            int[] status = new int[] { 0,0,0,0,0,0,0,0,0,0};

            checkBox1.Checked = (x1 == 1) ? true : false;
            status[0] = (x1 == 1) ? 1 : 0;
            checkBox2.Checked = (x2 == 1) ? true : false;
            status[1] = (x2 == 1) ? 1 : 0;
            checkBox3.Checked = (x3 == 1) ? true : false;
            status[2] = (x3 == 1) ? 1 : 0;
            checkBox4.Checked = (x4 == 1) ? true : false;
            status[3] = (x4 == 1) ? 1 : 0;
            checkBox5.Checked = (x5 == 1) ? true : false;
            status[4] = (x5 == 1) ? 1 : 0;
            checkBox6.Checked = (x6 == 1) ? true : false;
            status[5] = (x6 == 1) ? 1 : 0;
            checkBox7.Checked = (x7 == 1) ? true : false;
            status[6] = (x7 == 1) ? 1 : 0;
            checkBox8.Checked = (x8 == 1) ? true : false;
            status[7] = (x8 == 1) ? 1 : 0;
            checkBox9.Checked = (x9 == 1) ? true : false;
            status[8] = (x9 == 1) ? 1 : 0;
            checkBox10.Checked = (x10 == 1) ? true : false;
            status[9] = (x10 == 1) ? 1 : 0;



            string statusString = "";
            for (int i = 0; i < status.Length; i++)
            {
                statusString += status[i];
            }


            if (statusString != previousStatusString)
            {
                //send to server (only if the status has changed!)

                httpRequest("http://192.168.1.108/variables/variables.php?action=set&key=lightshow&api=f43g56rewrq4t5&value=" + statusString);

                previousStatusString = statusString;
            }
        }

        static void sw2(int x1, int x2, int x3, int x4, int x5, int x)
        {
            switchIt(0, false);
            if (x == 1)
            {
                switchIt(1, false);
            }
            else
            {
                switchIt(1, true);
            }

            
            if (x1 == 1)
            {
                switchIt(7, true);
            }
            else
            {
                switchIt(7, false);
            }

            if (x2 == 1)
            {
                switchIt(6, true);
            }
            else
            {
                switchIt(6, false);
            }

            if (x3 == 1)
            {
                switchIt(5, true);
            }
            else
            {
                switchIt(5, false);
            }

            if (x4 == 1)
            {
                switchIt(4, true);
            }
            else
            {
                switchIt(4, false);
            }

            if (x5 == 1)
            {
                switchIt(3, true);
            }
            else
            {
                switchIt(3, false);
            }


            try
            {
                serialPort1.Write(new byte[] { (byte)_bits }, 0, 1);
            }
            catch (Exception)
            {
                try
                {
                    serialPort1.Close();
                    serialPort1.Open();
                }
                catch (Exception) { }
            }
        }

        void otherDevices(int device, bool state) {
            switchItDevice(0, true);
            switchItDevice(device, state);

            try
            {
                serialPort1.Write(new byte[] { (byte)_device_bits }, 0, 1);
            }
            catch (Exception)
            {
                try
                {
                    serialPort1.Close();
                    serialPort1.Open();
                }
                catch (Exception) { }
            }
        }



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            //COM3
            System.ComponentModel.IContainer components = new System.ComponentModel.Container();
            serialPort1 = new System.IO.Ports.SerialPort(components);
            serialPort1.PortName = "COM3";
            serialPort1.BaudRate = 9600;

            serialPort1.Open();
            if (!serialPort1.IsOpen)
            {
                MessageBox.Show("Oops");
            }

            //Audio
            MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
            device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            tbMaster = (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            device.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);
            //System.ComponentModel.IContainer components = new System.ComponentModel.Container();
        }

        /* AUDIO 
         * ============================================================================================== */
        void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            if (this.InvokeRequired)
            {
                object[] Params = new object[1];
                Params[0] = data;
                this.Invoke(new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification), Params);
            }
            else
            {
                tbMaster = (int)(data.MasterVolume * 100);
            }
        }

        private void tbMaster_Scroll(object sender, EventArgs e)
        {
            device.AudioEndpointVolume.MasterVolumeLevelScalar = ((float)tbMaster / 100.0f);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (device!=null)
            {
                int value = (int)(device.AudioMeterInformation.MasterPeakValue * 100);



                label1.Text = value.ToString();

                //if (value > 40) value = 40;

                //int real = value * 10 / 40;
                


                int[] z = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
               
               //UV meter: 00111 11100
                if (mode == "music01110")
                {
                    int real = value / 20;
                    if (real > 0)
                    {

                        for (int i = 0; i < real; i++)
                        {
                            z[i] = 1;
                        }



                    }
                    else
                    {
                    }

                    s(z[4], z[3], z[2], z[1], z[0], z[0], z[1], z[2], z[3], z[4]);
                }


                if (mode == "music00100")
                {
                    
                    //UV meter 00010000
                      int real = value/10;
                      if (real > 0)
                      {
                         // for (int i = 0; i < real; i++)
                         // {
                        //      z[i] = 1;
                         // }
                          try
                          {
                              z[real] = 1;
                          }
                          catch (Exception) { }
                      }

                      s(z[0], z[1], z[2], z[3], z[4], z[5], z[6], z[7], z[8], z[9]);
                      
                    
                }

               

                //TEST s(1, 1, 1, 1, 0, 0, 1, 0, 0, 1);
                //TEST s(1, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            }


            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //get mode

            mode = httpRequest("http://192.168.1.108/variables/variables.php?action=get&key=lightshow_mode&api=f43g56rewrq4t5");

            if (mode == "knightrider")
            {
                modeTimer.Enabled = true;
            }
            else
            {
                modeTimer.Enabled = false;
            }

            if (mode == "manual")
            {
                string stat ="";
                //get status

                stat=httpRequest("http://192.168.1.108/variables/variables.php?action=get&key=lightshow&api=f43g56rewrq4t5");


                int[] z = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };


                for (int i = 0; i < stat.Length; i++)
                {
                    try
                    {
                        z[i] = Int32.Parse(stat[i].ToString());
                    }
                    catch (Exception) { }
                }
                
                s(z[0], z[1], z[2], z[3], z[4], z[5], z[6], z[7], z[8], z[9]);
            }

            label2.Text = mode;
        }

        private void modeTimer_Tick(object sender, EventArgs e)
        {
            if (mode == "knightrider")
            {
                int[] z = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                if (knightriderDirection == 0)
                {
                    knightrider++;

                    if (knightrider > 9)
                    {
                        knightrider = 8;
                        knightriderDirection = 1;
                    }
                }
                else
                {
                    knightrider--;

                    if (knightrider < 0)
                    {
                        knightrider = 2;
                        knightriderDirection = 0;
                    }
                }

                try
                {
                    z[knightrider] = 1;
                }
                catch (Exception) { }

                s(z[0], z[1], z[2], z[3], z[4], z[5], z[6], z[7], z[8], z[9]);
            }
        }

        private void light_Tick(object sender, EventArgs e)
        {
            string content = httpRequest("http://192.168.1.108/variables/variables.php?action=get&key=light&api=f43g56rewrq4t5");
            content = "on";
            if (content == "on")
            {
                otherDevices(CEILING_LIGHT, true);
            }
            else
            {
                otherDevices(CEILING_LIGHT, false);
            }
        }


    }
}
