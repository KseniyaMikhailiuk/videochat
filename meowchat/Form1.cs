using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using NAudio.Wave;

namespace meowchat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label1.Text = GetLocalIP();
            startToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = false;
        }
        
        private string GetLocalIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        public static string remoteAddress; // хост для отправки данных
        public static int FriendPort; // порт для отправки данных
        public static int MyPort; // локальный порт для прослушивания входящих подключений

       
        private VideoCaptureDevice WebCam;
        private FilterInfoCollection CamCollection;

        
        private void Form1_Load(object sender, EventArgs e)
        {
            CamCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            foreach (FilterInfo cam in CamCollection)
            {
                menuItem = new ToolStripMenuItem()
                {
                    Text = cam.Name,
                    Tag = cam.MonikerString
                };
                menuItem.Click += new EventHandler(MenuItemCamClickHandler);
                camToolStripMenuItem.DropDownItems.Add(menuItem);
            }
            camToolStripMenuItem.Enabled = false;
        }

        private void MenuItemCamClickHandler(object sender, EventArgs e)
        {
            camToolStripMenuItem.Enabled = false;
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            try
            {
                WebCam = new VideoCaptureDevice(clickedItem.Tag.ToString());
                WebCam.NewFrame += WebCam_NewFrame;
                WebCam.Start();
                startToolStripMenuItem.Enabled = true;
                camToolStripMenuItem.Enabled = false;
            }
            catch(Exception ex)
            {
                MessageBoxButtons button = MessageBoxButtons.OK;
                string caption = "Error";
                MessageBox.Show(ex.Message, caption, button);
            }
        }

        private void MenuItemMicroClickHandler(object sender, EventArgs e)
        {
            microToolStripMenuItem.Enabled = false;
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            try
            {
                SendReceiveDisplayProcess.input.DeviceNumber = (int)clickedItem.Tag;
            }
            catch (Exception ex)
            {
                MessageBoxButtons button = MessageBoxButtons.OK;
                string caption = "Error";
                MessageBox.Show(ex.Message, caption, button);
            }
        }

        private void MenuItemOutputClickHandler(object sender, EventArgs e)
        {
            outputToolStripMenuItem.Enabled = false;
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            try
            {
                SendReceiveDisplayProcess.output.DeviceNumber = (int)clickedItem.Tag;
            }
            catch (Exception ex)
            {
                MessageBoxButtons button = MessageBoxButtons.OK;
                string caption = "Error";
                MessageBox.Show(ex.Message, caption, button);
            }
        }

        public Image tempImage;
        private void WebCam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap bitmap = new Bitmap((Bitmap)eventArgs.Frame.Clone(), pictureBoxMine.Size);
                SendReceiveDisplayProcess.TempImage = bitmap.Clone() as Image;
                SendReceiveDisplayProcess._semaphoreSlim.Wait();

                pictureBoxMine.Image = bitmap;
                SendReceiveDisplayProcess._semaphoreSlim.Release(1);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                MessageBoxButtons button = MessageBoxButtons.OK;
                string caption = "Error";
                MessageBox.Show(ex.Message, caption, button);
            }
        }

        Thread sendThread = new Thread(new ThreadStart(SendReceiveDisplayProcess.SendMessage));
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SendReceiveDisplayProcess.input.WaveFormat = new WaveFormat(8000, 16, 1);
                SendReceiveDisplayProcess.input.DataAvailable += SendReceiveDisplayProcess.Voice_Input;
                SendReceiveDisplayProcess.input.StartRecording();
                sendThread.Start();
                WebCam.Start();
                stopToolStripMenuItem.Enabled = true;
                startToolStripMenuItem.Enabled = false;
            }
            catch(Exception ex)
            {
                MessageBoxButtons button = MessageBoxButtons.OK;
                string caption = "Error";
                MessageBox.Show(ex.Message, caption, button);
            }
        }

        private void Abort()
        {
            if (WebCam!= null)
            {
                WebCam.SignalToStop();
                WebCam.WaitForStop();
                WebCam = null;
            }
            if (SendReceiveDisplayProcess.listeningSocket != null)
            {
                SendReceiveDisplayProcess.listeningSocket.Close();
                SendReceiveDisplayProcess.listeningSocket.Dispose();
            }
            if (SendReceiveDisplayProcess.client != null)
            {
                SendReceiveDisplayProcess.client.Close();
                SendReceiveDisplayProcess.client.Dispose();
            }


            if (SendReceiveDisplayProcess.output != null)
            {
                SendReceiveDisplayProcess.output.Stop();
                SendReceiveDisplayProcess.output.Dispose();
                SendReceiveDisplayProcess.output = null;
            }
            if (SendReceiveDisplayProcess.input != null)
            {
                SendReceiveDisplayProcess.input.Dispose();
                SendReceiveDisplayProcess.input = null;
            }

        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WebCam.IsRunning)
            {
                WebCam.Stop();
                WebCam.WaitForStop();
            }
            if (SendReceiveDisplayProcess.input != null)
            {
                SendReceiveDisplayProcess.input.StopRecording();
            }
            Graphics g = pictureBoxMine.CreateGraphics();
            g.Clear(Color.White);
            startToolStripMenuItem.Enabled = false;
        }

        public static bool isStopped = false;

        Thread receiveThread = new Thread(new ParameterizedThreadStart(SendReceiveDisplayProcess.ReceiveMessage));
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if ((new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")).IsMatch(textBoxServerIP.Text) &&
                    (new Regex(@"\b[0-9]+\b")).IsMatch(textBoxMyPort.Text) &&
                    (new Regex(@"\b[0-9]+\b")).IsMatch(textBoxFriendPort.Text))
            {
                try
                {
                    MyPort = Int32.Parse(textBoxMyPort.Text);
                    remoteAddress = textBoxServerIP.Text; // адрес, к которому мы подключаемся
                    FriendPort = Int32.Parse(textBoxFriendPort.Text); // порт, к которому мы подключаемся

                    string infoMessage = "Аминь";
                    string caption = "bless and save";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(infoMessage, caption, buttons);

                    camToolStripMenuItem.Enabled = true;
                    outputToolStripMenuItem.Enabled = true;
                    SendReceiveDisplayProcess sendReceiveDisplayProcess = new SendReceiveDisplayProcess(remoteAddress,
                                                        FriendPort, MyPort);
                    


                    SendReceiveDisplayProcess.input = new WaveIn();

                    int waveInDevices = WaveIn.DeviceCount;
                    ToolStripMenuItem menuItem = new ToolStripMenuItem();
                    for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
                    {
                        WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                        menuItem = new ToolStripMenuItem()
                        {
                            Text = deviceInfo.ProductName,
                            Tag = waveInDevice
                        };
                        menuItem.Click += new EventHandler(MenuItemMicroClickHandler);
                        microToolStripMenuItem.DropDownItems.Add(menuItem);
                    }
                    microToolStripMenuItem.Enabled = true;

                    SendReceiveDisplayProcess.output = new WaveOut();
                    int waveOutDevices = WaveOut.DeviceCount;
                    menuItem = new ToolStripMenuItem();
                    for (int waveOutDevice = 0; waveOutDevice < waveOutDevices; waveOutDevice++)
                    {
                        WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveOutDevice);
                        menuItem = new ToolStripMenuItem()
                        {
                            Text = deviceInfo.ProductName,
                            Tag = waveOutDevice
                        };
                        menuItem.Click += new EventHandler(MenuItemOutputClickHandler);
                        outputToolStripMenuItem.DropDownItems.Add(menuItem);
                    }
                    outputToolStripMenuItem.Enabled = true;
                    receiveThread.Start(this);
                    buttonConnect.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBoxButtons button = MessageBoxButtons.OK;
                    string caption = "Error";
                    MessageBox.Show(ex.Message, caption, button);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isStopped = true;
            Thread.Sleep(500);
            Abort();
        }
    }
}
