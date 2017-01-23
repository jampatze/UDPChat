using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDPServer
{
    /// <summary>
    /// A simple UDP-server client that echoes messages sent to all chat clients
    /// </summary>
    class UDPServer
    {
        // Holds the info of client connected to server
        struct ClientInfo
        {
            public EndPoint endpoint;
            public string name;
        }

        List<ClientInfo> clients = new List<ClientInfo>();

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            int port = 9999;
            byte[] byteData = new byte[1024];
            Socket serverSocket;
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, port);

            // try to boot up the server
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                serverSocket.Bind(iep);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Waiting for clients...");

            // recieve data
            while (!Console.KeyAvailable)
            {

                // identify incoming cilents
                IPEndPoint iepSender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint epSender = (EndPoint)iepSender;

                // recieve data
                serverSocket.ReceiveFrom(byteData, ref epSender);

                if (clients)
            }
        }

        /// <summary>
        /// Parses the AsyncCallback and prints it
        /// </summary>
        /// <param name="asyncResult"></param>
        private void OnRecieve(IAsyncResult asyncResult)
        {
            try
            {
                byte[] data;

                CustomPacket recievedData = new CustomPacket(this.byteData);
                CustomPacket sendData = new CustomPacket();
                IPEndPoint clients = new IPEndPoint(IPAddress.Any, 0);
                EndPoint epSender = (EndPoint)clients;

                serverSocket.EndReceiveFrom(asyncResult, ref epSender);

                sendData.cmdCommand = recievedData.cmdCommand;
                sendData.strName = recievedData.strName;

                switch (recievedData.cmdCommand)
                {
                    case Command.Message:
                        sendData.strMessage = string.Format("{0}: {1}", recievedData.strName, recievedData.strMessage);
                        break;

                    case Command.Login:
                        ClientInfo client = new ClientInfo();
                        client.endpoint = epSender;
                        client.name = recievedData.strName;
                        this.clients.Add(client);
                        sendData.strMessage = string.Format(">>>{0} has logged in<<<", recievedData.strName);
                        break;

                    case Command.Logout:
                        foreach (ClientInfo c in this.clients)
                        {
                            if (c.endpoint.Equals(epSender))
                            {
                                this.clients.Remove(c);
                                break;
                            }
                        }
                        sendData.strMessage = string.Format(">>>{0} has logged in<<< ", recievedData.strName);
                        break;
                }

                data = sendData.ToByte();

                foreach (ClientInfo c in this.clients)
                {
                    if (c.endpoint != epSender ||sendData.cmdCommand != Command.Login)
                    {
                        serverSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, c.endpoint, new AsyncCallback(this.SendData), c.endpoint);
                    }
                }

                // Continue listening
                serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epSender, new AsyncCallback(OnRecieve), epSender);
                // print the chat message:
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while recieving:" + ex);
            }
        }


        /// <summary>
        /// Sends data
        /// </summary>
        /// <param name="asyncResult"></param>
        public void SendData(IAsyncResult asyncResult)
        {
            try
            {
                serverSocket.EndSend(asyncResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendData error:" + ex.Message);
            }
        }
    }
}
