using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using NAudio.Wave;


namespace meowchat
{
    public class SendReceiveDisplayProcess
    {
        public static Image TempImage;
        public static string RemoteAddress; // хост для отправки данных
        public static int FriendPort; // порт для отправки данных
        public static int MyPort; // локальный порт для прослушивания входящих подключений
        public static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        //поток для нашей речи
        public static WaveIn input = new WaveIn();
        //поток для речи собеседника
        public static WaveOut output;

        //буфферный поток для передачи через сеть
        public static BufferedWaveProvider bufferStream;
        //поток для прослушивания входящих сообщений
        public static Thread in_thread;

        //сокет для приема (протокол UDP)
        public static Socket listeningSocket;
        //сокет отправитель
        public static UdpClient client = new UdpClient();



        public SendReceiveDisplayProcess(string remoteAddress, int friendPort, int myPort)
        {
            RemoteAddress = remoteAddress;
            FriendPort = friendPort;
            MyPort = myPort;
        }

        public static void SendMessage()
        {
            byte[] byteBuffer;
            try
            {
                while (!Form1.isStopped)
                {
                    if (TempImage != null)
                    {
                        using (UdpClient sender = new UdpClient())
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                Image image = new Bitmap(TempImage, 150, 150);
                                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                byteBuffer = ms.ToArray();
                                Array.Resize(ref byteBuffer, byteBuffer.Length + 1);
                                byteBuffer[byteBuffer.Length - 1] = 0;
                                sender.Send(byteBuffer, byteBuffer.Length, RemoteAddress, FriendPort); // отправка
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBoxButtons button = MessageBoxButtons.OK;
                string caption = "Error";
                MessageBox.Show(ex.Message, caption, button);
            }
        }

        public static void Voice_Input(object sender, WaveInEventArgs e)
        {
            try
            {
                if (!Form1.isStopped)
                {
                    //посылаем байты, полученные с микрофона на удаленный адрес
                    byte[] data = e.Buffer;
                    Array.Resize(ref data, data.Length + 1);
                    data[data.Length - 1] = 1;
                    client.Send(data, data.Length, RemoteAddress, FriendPort);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void ReceiveMessage(object form)
        {        
            output = new WaveOut();
            bufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            output.Init(bufferStream);
            output.Play();
            using (UdpClient receiver = new UdpClient(MyPort))
            {
                IPEndPoint remoteIp = null; // адрес входящего подключения;
                try
                {
                    while (!Form1.isStopped)
                    {
                        try
                        {
                            var form1 = (Form1)form;
                            Image image;
                            if (receiver.Available > 0)
                            {
                                byte[] data = receiver.Receive(ref remoteIp);
                                if (data[data.Length - 1] == 0)
                                {
                                   
                                    using (var ms = new MemoryStream(data))
                                    {
                                        image = Image.FromStream(ms);
                                    }
                                    _semaphoreSlim.Wait();
                                    form1.pictureBoxFriend.Image = image;
                                    _semaphoreSlim.Release(1);
                                }
                                else
                                {
                                    if (data[data.Length - 1] == 1)
                                    {
                                        Array.Resize(ref data, data.Length - 1);
                                        bufferStream.AddSamples(data, 0, data.Length);
                                        output.Play();
                                        //bufferStream.ClearBuffer();
                                    }
                                }
                            }
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }
                        catch (Exception ex)
                        {
                            MessageBoxButtons button = MessageBoxButtons.OK;
                            string caption = "Error";
                            MessageBox.Show(ex.Message, caption, button);
                        }
                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    //MessageBoxButtons button = MessageBoxButtons.OK;
                    //string caption = "Error";
                    //MessageBox.Show(ex.Message, caption, button);
                }
            } 
               
        }
    }
}
