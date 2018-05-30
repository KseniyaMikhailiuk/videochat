using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using NAudio;
using NAudio.Wave;
using System.IO;

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

        public static int AudioFriendPort; 
        public static int AudioMyPort;


        //поток для нашей речи
        WaveIn input;
        //поток для речи собеседника
        WaveOut output;

        //буфферный поток для передачи через сеть
        BufferedWaveProvider bufferStream;
        //поток для прослушивания входящих сообщений
        Thread in_thread;

        //сокет для приема (протокол UDP)
        Socket listeningSocket;
        //сокет отправитель
        Socket client;



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
                sendThread.Start();
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
            if (listeningSocket != null)
            {
                listeningSocket.Close();
                listeningSocket.Dispose();
            }
            if (client != null)
            {
            client.Close();
            client.Dispose();
            }


            if (output != null)
            {
                output.Stop();
                output.Dispose();
                output = null;
            }
            if (input != null)
            {
                input.Dispose();
                input = null;
            }

        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WebCam.IsRunning)
            {
                WebCam.Stop();
                WebCam.WaitForStop();
                WebCam = null;
            }
            Graphics g = pictureBoxMine.CreateGraphics();
            g.Clear(Color.White);
            startToolStripMenuItem.Enabled = true;
        }

        public static bool isStoped = false;

        Thread receiveThread = new Thread(new ParameterizedThreadStart(SendReceiveDisplayProcess.ReceiveMessage));
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if ((new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")).IsMatch(textBoxServerIP.Text) &&
                    (new Regex(@"\b[0-9]+\b")).IsMatch(textBoxMyPort.Text) &&
                    (new Regex(@"\b[0-9]+\b")).IsMatch(textBoxFriendPort.Text))
            {
                try
                {
                    AudioMyPort = Int32.Parse(audioMyPort.Text);
                    AudioFriendPort = Int32.Parse(audioFriendPort.Text);
                    MyPort = Int32.Parse(textBoxMyPort.Text);
                    remoteAddress = textBoxServerIP.Text; // адрес, к которому мы подключаемся
                    FriendPort = Int32.Parse(textBoxFriendPort.Text); // порт, к которому мы подключаемся

                    string infoMessage = "Аминь";
                    string caption = "bless and save";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(infoMessage, caption, buttons);
                    camToolStripMenuItem.Enabled = true;
                    SendReceiveDisplayProcess sendReceiveDisplayProcess = new SendReceiveDisplayProcess(remoteAddress,
                                                        FriendPort, MyPort);
                    receiveThread.Start(this);

                    //сокет для отправки звука
                    client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    input = new WaveIn();
                    input.WaveFormat = new WaveFormat(8000, 16, 1);
                    input.DataAvailable += Voice_Input;
                    output = new WaveOut();
                    bufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
                    output.Init(bufferStream);
                    //создаем поток для прослушивания
                    in_thread = new Thread(new ThreadStart(Listening));
                    //запускаем его
                    in_thread.Start();
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

        private void Voice_Input(object sender, WaveInEventArgs e)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(remoteAddress);
                //Подключаемся к удаленному адресу
                IPEndPoint remote_point = new IPEndPoint(ip, AudioFriendPort);
                //посылаем байты, полученные с микрофона на удаленный адрес
                client.SendTo(e.Buffer, remote_point);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Listening()
        {
            //Прослушиваем по адресу
            IPEndPoint localIP = new IPEndPoint(IPAddress.Parse(GetLocalIP()), AudioMyPort);
            listeningSocket.Bind(localIP);
            //начинаем воспроизводить входящий звук
            output.Play();
            //адрес, с которого пришли данные
            EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);
            //бесконечный цикл
            while (!isStoped)
            {
                try
                {
                    //промежуточный буфер
                    byte[] data = new byte[65000];
                    //получено данных
                    if (listeningSocket.Available > 0)
                    {
                        int received = listeningSocket.ReceiveFrom(data, ref remoteIp);
                        //добавляем данные в буфер, откуда output будет воспроизводить звук
                        bufferStream.AddSamples(data, 0, received);
                        bufferStream.ClearBuffer();
                    }

                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isStoped = true;
            Thread.Sleep(1500);
            Abort();
        }
    }
}
