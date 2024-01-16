using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace crosses
{
    class Client
    {

        bool connected;
        CircleQueue<string> messages = new CircleQueue<string>(5);


        public void ExecuteClient(string displayName)
        {

            try
            {

                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

                Socket sender = new Socket(ipAddr.AddressFamily,
                           SocketType.Stream, ProtocolType.Tcp);

                try
                {

                    sender.Connect(localEndPoint);


                    Console.SetCursorPosition(0, 20);
                    messages.Add("Connected!");
                    DrawScreen();

                    connected = true;

                    Message registerMessage = new Message(MessageType.REGISTER, displayName);
                    SendMessage(sender, registerMessage.Serialise());
                    Thread sendThread = new Thread(() => HandleSending(sender));
                    sendThread.Start();
                    Thread receiveThread = new Thread(() => HandleReceiving(sender));
                    receiveThread.Start();

                    sendThread.Join();
                    receiveThread.Join();

                    // Close Socket using 
                    // the method Close()
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }

                // Manage of Socket's Exceptions
                catch (ArgumentNullException ane)
                {

                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {

                    Console.WriteLine("SocketException : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }

            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }

        private static Message ReceiveMessage(Socket client)
        {

            byte[] bytes = new Byte[1024];
            string data = null;

            while (true)
            {

                int numByte = client.Receive(bytes);

                data += Encoding.ASCII.GetString(bytes,
                                           0, numByte);

                if (data.IndexOf("<EOF>") > -1)
                    return Message.Deserialise(data);
            }
        }

        private static void SendMessage(Socket client, byte[] message)
        {
            int bytesSent = 0;
            while (bytesSent < message.Length)
            {
                bytesSent += client.Send(message, bytesSent, message.Length - bytesSent, SocketFlags.None);
            }
        }

        private void HandleSending(Socket socket) {
            
            while (connected) {
                string input = Console.ReadLine();
                Message message;
                if (input == "<DONE>")
                {
                    message = new Message(MessageType.LEAVE, input);
                    connected = false;
                }
                else {
                    message = new Message(MessageType.TEXT, input);
                }
                
                byte[] messageSent = message.Serialise();
                SendMessage(socket, messageSent);
                
            }
        }

        private void HandleReceiving(Socket socket) {
            while (connected)
            {
                Message message = ReceiveMessage(socket);
                messages.Add(message.content);

                DrawScreen();
                

                if (message.type == MessageType.CLOSE)
                {
                    connected = false;
                }
            }
        }


        private void DrawScreen() {
            int x = Console.CursorLeft;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            foreach (string text in messages.Iter())
            {
                Console.WriteLine(text);
            }
            Console.SetCursorPosition(x, messages.Length + 2);
        }
    }
}
