using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace crosses
{
    class Server
    {

        ThreadSafeList<Socket> sockets;
        
        public Server()
        {
            sockets = new ThreadSafeList<Socket>();
        }

        public void Start()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

            // Creation TCP/IP Socket using 
            // Socket Class Constructor
            Socket listener = new Socket(ipAddr.AddressFamily,
                         SocketType.Stream, ProtocolType.Tcp);

            try
            {

                listener.Bind(localEndPoint);

                listener.Listen(10);

                while (true)
                {

                    Console.WriteLine("Waiting connection ... ");

                    Socket clientSocket = listener.Accept();

                    Thread clientThread = new Thread(() => HandleClient(clientSocket));
                    clientThread.Start();

                    //clientThread.Join();

                    
                }
            }

            catch (SocketException e) {
                Console.WriteLine(e.ToString()); // potential lost connection
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static Message ReceiveMessage(Socket client) {

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

        private static void SendMessage(Socket client, byte[] message) {
            int bytesSent = 0;
            while (bytesSent < message.Length)
            {
                bytesSent += client.Send(message, bytesSent, message.Length - bytesSent, SocketFlags.None);
            }
        }

        private void HandleClient(Socket client) {
            bool connected = true;

            sockets.Add(client);
            string clientName = "";

            Message registerMessage = ReceiveMessage(client);
            if (registerMessage.type == MessageType.REGISTER)
            {
                clientName = registerMessage.content;
                Message messageOut = new Message(MessageType.TEXT, clientName + " joined the chat");
                byte[] lmessage = messageOut.Serialise();
                sockets.Map(c => SendMessage(c, lmessage));
            }
            else 
            {
                Message messageOut = new Message(MessageType.CLOSE, "No register message found");
                connected = false;
            }

            while (connected) {
                Message messageIn = ReceiveMessage(client);
                if (messageIn.type == MessageType.LEAVE)
                {
                    Message leaveMessage = new Message(MessageType.TEXT, clientName + " left the chat");
                    byte[] lmessage = leaveMessage.Serialise();
                    sockets.Map(c => SendMessage(c, lmessage));
                    connected = false;
                    break;
                }

                Console.WriteLine("Text received -> {0} ", messageIn.content);
                Message messageOut = new Message(MessageType.TEXT, clientName + "> " + messageIn.content);
                byte[] message = messageOut.Serialise();

                sockets.Map(c => SendMessage(c, message));
            }

            sockets.Remove(client);
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        
    }
}
