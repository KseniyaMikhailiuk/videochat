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
    public class SendReceiveDisplayProcess
    {
        public static Image TempImage;
        public static string RemoteAddress; // хост для отправки данных
        public static int FriendPort; // порт для отправки данных
        public static int MyPort; // локальный порт для прослушивания входящих подключений
        public static object locker = new object();

        public SendReceiveDisplayProcess(string remoteAddress, int friendPort, int myPort)
        {
            RemoteAddress = remoteAddress;
            FriendPort = friendPort;
            MyPort = myPort;
        }

        public static void SendMessage()
        {
            UdpClient sender = new UdpClient(); // создаем UdpClient для отправки сообщений
            try
            {
                while (true)
                {
                    if (TempImage != null)
                    {
                        MemoryStream ms = new MemoryStream();
                        lock (locker)
                        {
                            TempImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            byte[] byteBuffer = ms.ToArray();
                            sender.Send(byteBuffer, byteBuffer.Length, RemoteAddress, FriendPort); // отправка
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

        public static void ReceiveMessage(object form)
        {
            UdpClient receiver = new UdpClient(MyPort); // UdpClient для получения данных
            IPAddress ip = IPAddress.Parse(RemoteAddress);
            byte[] ipbytes = ip.GetAddressBytes();

            IPEndPoint remoteIp = new IPEndPoint(ip, FriendPort); // адрес входящего подключения;
            try
            {
                while (true)
                {
                    if (receiver.Receive(ref remoteIp) != null)
                    {
                        lock (locker)
                        {
                            
                            try
                            {
                                Form1 form1 = (Form1)form;
                                Image image;
                                byte[] data = receiver.Receive(ref remoteIp);
                                using (var ms = new MemoryStream(data))
                                {
                                    image = Image.FromStream(ms);
                                }
                                form1.pictureBoxtempFriend.Image = image;
                                //image.Dispose();
                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                            }
                            catch (Exception ex)
                            {
                                MessageBoxButtons button = MessageBoxButtons.OK;
                                string caption = "Error";
                                MessageBox.Show("эта ебучая ошибка где-то здесь2", caption, button);
                            }
                        }
                        Thread.Sleep(100);
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
    }
}
