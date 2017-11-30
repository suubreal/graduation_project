using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SBL
{
    class Client
    {
        TcpClient clientSocket = new TcpClient();

        public void Start()
        {
            new Thread(delegate()
            {
                InitSocket();
            }).Start();
        }

        private void InitSocket()
        {
            try
            {
                //System.Diagnostics.Debug.WriteLine("hello");
                Console.WriteLine("Client Socket Program - Server Connected ...");
                Trace.WriteLine("text");

                clientSocket.Connect("127.0.0.1", 7001);
            }

            catch (SocketException se)
            {   //server가 켜져 있지 않을 때
                Console.WriteLine("1");
                MessageBox.Show(se.Message, "Error1");
            }
            catch (Exception ex)
            {
                Console.WriteLine("2");
                MessageBox.Show(ex.Message, "Error2");
            }
        }

        // 이거 전체 폼 꺼질때 해줘야하는데 어디서 해야할지 모르겠당
        private void MainForm_FormClosing(object sender, CancelEventArgs e)
        {
            if (clientSocket != null)
                clientSocket.Close();
        }

        public void sendMsg(String msg)
        {
            byte[] tmpbuffer = Encoding.UTF8.GetBytes(msg);
            byte[] sizebuffer = BitConverter.GetBytes(tmpbuffer.Length);
            Console.WriteLine("msg : " + msg);
            Console.WriteLine("length : " + sizebuffer.Length);

            NetworkStream stream = clientSocket.GetStream();
            Console.WriteLine("sending...");

            //size
            stream.Write(sizebuffer, 0, sizebuffer.Length);
            stream.Flush();

            //content
            stream.Write(tmpbuffer, 0, tmpbuffer.Length);
            Console.WriteLine("sendMsg - " + msg);
            stream.Flush();

            //stream.Close();
            //clientSocket.Close();
        }

        public void sendFile(String fileName)
        {
            NetworkStream stream = clientSocket.GetStream();
            FileStream fileStr = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            Console.WriteLine("sending...");

            //size
            int size = (int)fileStr.Length;
            byte[] sizebuffer = BitConverter.GetBytes(size);
            stream.Write(sizebuffer, 0, sizebuffer.Length);
            Console.WriteLine("size : " + size);

            //file 
            BinaryReader binReader = new BinaryReader(fileStr);
            sizebuffer = binReader.ReadBytes(size);
            stream.Write(sizebuffer, 0, sizebuffer.Length);

            binReader.Close();
            Console.WriteLine("file sended!!!!");
            
            clientSocket.Client.SendFile(fileName);
        }
        
        public string recvMsg()
        {
            Boolean flag = true;
            Console.WriteLine("Receiving....");
            NetworkStream stream = clientSocket.GetStream();
            StreamReader reader = new StreamReader(stream);
            string default_s = "default";

            while (flag) { 
                //size
                byte[] buff = new byte[4];
                int k = stream.Read(buff, 0, 4);
                int size = BitConverter.ToInt32(buff, 0);
                stream.Flush();

                if (size > 0) {
                    flag = false;
                    //string
                    buff = new byte[size];
                    k = stream.Read(buff, 0, size);
                    string decoded_s = Encoding.UTF8.GetString(buff);
                    stream.Flush();
                    Console.WriteLine(">recv : " + decoded_s);
                    //string decoded_i = Encoding.UTF8.GetString(buff);
                    //Console.WriteLine("original : " + BitConverter.ToInt32(buff, 0));//.ToString(buff));
                    //Console.WriteLine("decoded_s :" + decoded_s);
                    /* Console.Write("Decoded chars: ");
                     foreach (Char c in chars)
                     {
                         Console.Write("[{0}]", c);
                     }*/
                    return decoded_s;
                }
            }
            return default_s;
        }

    }
}
