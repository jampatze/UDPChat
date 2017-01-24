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
    /// Why is this implemented as a command line program?
    /// I know it's not the best way, but I just wanted to.
    /// </summary>
    class UDPServer
    {
        // Holds the info of a single client connected to server
        struct ClientInfo
        {
            public EndPoint endpoint;
            public string name;
        }


        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            int port = 9999;
            byte[] byteData = new byte[1024];
            Socket serverSocket;
            List<ClientInfo> clients = new List<ClientInfo>();
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
            // ATTN! needs to recieve something from a client before the server can be closed
            while (!Console.KeyAvailable)
            {

                // identify incoming cilents
                IPEndPoint iepSender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint epSender = (EndPoint)iepSender;

                // recieve data
                serverSocket.ReceiveFrom(byteData, ref epSender);

                // when data is recieved
                byte[] data;

                CustomPacket recievedData = new CustomPacket(byteData);
                CustomPacket sendData = new CustomPacket();
                IPEndPoint clientsIEP = new IPEndPoint(IPAddress.Any, 0);
                EndPoint epSenders = (EndPoint)clientsIEP;

                sendData.cmdCommand = recievedData.cmdCommand;
                sendData.strName = recievedData.strName;

                // parse the recieved data
                switch (recievedData.cmdCommand)
                {
                    case Command.Message:
                        sendData.strMessage = string.Format("{0}: {1}", recievedData.strName, recievedData.strMessage);
                        Console.WriteLine(string.Format("{0}: {1}", recievedData.strName, recievedData.strMessage));
                        break;

                    case Command.Login:
                        ClientInfo client = new ClientInfo();
                        client.endpoint = epSender;
                        client.name = recievedData.strName;
                        clients.Add(client);
                        sendData.strMessage = string.Format(">>>{0} has logged in<<<", recievedData.strName);
                        Console.WriteLine(string.Format(">>>{0} has logged in<<<", recievedData.strName));
                        break;

                    case Command.Logout:
                        foreach (ClientInfo c in clients)
                        {
                            if (c.endpoint.Equals(epSender))
                            {
                                clients.Remove(c);
                                break;
                            }
                        }
                        sendData.strMessage = string.Format(">>>{0} has logged out<<< ", recievedData.strName);
                        Console.WriteLine(string.Format(">>>{0} has logged out<<< ", recievedData.strName));
                        break;
                }

                data = sendData.ToByte();

                // send the message to clients
                foreach (ClientInfo c in clients)
                {
                    try
                    {
                        if (c.endpoint != epSender || sendData.cmdCommand != Command.Login)
                        {
                            serverSocket.SendTo(sendData.ToByte(), c.endpoint);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("SendData error:" + ex.Message);
                        break;
                    }
                }

            }

            serverSocket.Close();
        }
    }
}
