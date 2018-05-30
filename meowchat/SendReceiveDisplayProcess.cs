using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using NAudio;
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


        public SendReceiveDisplayProcess(string remoteAddress, int friendPort, int myPort)
        {
            RemoteAddress = remoteAddress;
            FriendPort = friendPort;
            MyPort = myPort;
        }

        public static void SendMessage()
        {

            try
            {
                while (!Form1.isStoped)
                {
                    if (TempImage != null)
                    {
                        using (UdpClient sender = new UdpClient())
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                byte[] byteBuffer;
                                TempImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
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

        public static void ReceiveMessage(object form)
        {
            using (UdpClient receiver = new UdpClient(MyPort))
            {
                //IPAddress ip = IPAddress.Parse(RemoteAddress);
                IPEndPoint remoteIp = null; // адрес входящего подключения;
                try
                {
                    while (!Form1.isStoped)
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
                        Thread.Sleep(50);
                    }
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
