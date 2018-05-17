using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;

namespace meowchat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //PictureBox pictureBox = new PictureBox();
            pictureBoxtempFriend = pictureBoxFriend;
            startToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = false;
        }
        
        public static string remoteAddress; // хост для отправки данных
        public static int FriendPort; // порт для отправки данных
        public static int MyPort; // локальный порт для прослушивания входящих подключений
        public PictureBox pictureBoxtempFriend;


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
            lock (SendReceiveDisplayProcess.locker)
            {
                try
                {
                    Bitmap bitmap = new Bitmap((Bitmap)eventArgs.Frame.Clone(), pictureBoxMine.Size);
                    SendReceiveDisplayProcess.TempImage = bitmap.Clone() as Image;
                    pictureBoxMine.Image = bitmap;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch(Exception ex)
                {
                    MessageBoxButtons button = MessageBoxButtons.OK;
                    string caption = "Error";
                    MessageBox.Show(ex.Message, caption, button);

                }
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
            if (receiveThread.IsAlive)
            {
                receiveThread.Abort();
                receiveThread = null;
                Thread.Sleep(200);
            }
            if (sendThread.IsAlive)
            {
                sendThread.Abort();
                Thread.Sleep(200);
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Abort();
        }

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
                    SendReceiveDisplayProcess SendReceiveDisplayProcess = new SendReceiveDisplayProcess(remoteAddress,
                                                        FriendPort, MyPort);
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
    }
}
