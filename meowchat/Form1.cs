using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;


namespace meowchat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PictureBox pictureBox = new PictureBox();
            startToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = false;
        }
        
        static string remoteAddress; // хост для отправки данных
        static int FriendPort; // порт для отправки данных
        static int MyPort; // локальный порт для прослушивания входящих подключений

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
        //Thread sendThread = new Thread(new ParameterizedThreadStart(SendMessage));
        private void SendMessage(/*object form*/)
        {
            UdpClient sender = new UdpClient(); // создаем UdpClient для отправки сообщений
            try
            {
                //Form1 form1 = (Form1)form;
                while (stopToolStripMenuItem.Pressed)// && form1.pictureBoxMine.Image != null)
                {
                    if (pictureBoxMine.Image != null)
                    { 
                        string data = JsonConvert.SerializeObject(new Bitmap(pictureBoxMine.Image), new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.All
                        });
                        string readyString = Convert.ToBase64String(Encoding.Default.GetBytes(data));
                        try
                        {
                            sender.Send(Encoding.Default.GetBytes(readyString), readyString.Length, remoteAddress, FriendPort); // отправка
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sender.Close();
            }
        }

        private static void ReceiveMessage(object form)
        {
            UdpClient receiver = new UdpClient(MyPort); // UdpClient для получения данных
            IPAddress ip = IPAddress.Parse(remoteAddress);
            byte[] ipbytes = ip.GetAddressBytes();

            IPEndPoint remoteIp = new IPEndPoint(ip, FriendPort); // адрес входящего подключения
            try
            {
                Form1 form1 = (Form1)form;
                while (!form1.stopToolStripMenuItem.Pressed)
                {
                    try
                    {
                        byte[] data = receiver.Receive(ref remoteIp);
                        string message = Encoding.ASCII.GetString(data);
                        byte[] result = Convert.FromBase64String(message);
                        Image image = JsonConvert.DeserializeObject<Bitmap>(Encoding.Default.GetString(result),
                            new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.All
                            });

                        form1.pictureBoxFriend.Image = image;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                receiver.Close();
            }
        }

        private void MenuItemCamClickHandler(object sender, EventArgs e)
        {
            camToolStripMenuItem.Enabled = false;
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            try
            {
                WebCam = new VideoCaptureDevice(clickedItem.Tag.ToString());
                if (!WebCam.IsRunning)
                {
                    WebCam.NewFrame += WebCam_NewFrame;
                    WebCam.Start();
                    startToolStripMenuItem.Enabled = true;
                    camToolStripMenuItem.Enabled = false;
                }
            }
            catch(Exception ex)
            {
                MessageBoxButtons button = MessageBoxButtons.OK;
                string caption = "Error";
                MessageBox.Show(ex.Message, caption, button);
            }

        }

        private void WebCam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = new Bitmap((Bitmap)eventArgs.Frame.Clone(), pictureBoxMine.Size);
            pictureBoxMine.Image = bitmap/*(Image)eventArgs.Frame.Clone()*/;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }


        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //WebCam.NewFrame += WebCam_NewFrame;
            //WebCam.Start();
            stopToolStripMenuItem.Enabled = true;
            startToolStripMenuItem.Enabled = false;
            //sendThread.Start(this);
            SendMessage(/*this*/);
        }

        private void Abort()
        {
            if (WebCam!= null)
            {
                WebCam.Stop();
            }
            if (receiveThread.IsAlive)
            {
                receiveThread.Abort();
            }
            //if (sendThread.IsAlive)
            //{
            //    sendThread.Abort();
            //}
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WebCam.IsRunning)
            {
                WebCam.Stop();
            }
            Graphics g = pictureBoxMine.CreateGraphics();
            g.Clear(Color.White);
            startToolStripMenuItem.Enabled = true;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Abort();
        }

        Thread receiveThread = new Thread(new ParameterizedThreadStart(ReceiveMessage));
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
