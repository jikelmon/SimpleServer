using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkingLib.TCP
{
    public delegate void TcpServerEventIncomingConnection(RemoteClient remote_endpoint);
    public delegate void TcpServerEvent();

    public class TcpServer
    {
        Socket server_socket { get; set; }
        IPEndPoint local_endpoint { get; set; }
        public int port { get; set; }
        public int buffer_size {
            get { return buffer_size; }
            set
            {
                this.buffer_size = value;
                receive_buffer = new byte[buffer_size];
            }
        }

        byte[] receive_buffer = null;

        public event TcpServerEvent ServerStarted;
        public event TcpServerEventIncomingConnection IncomingConnection;

        public TcpServer(IPVersion type, int port = 56277, int buffer_size = 2048)
        {
            this.port = port;
            this.buffer_size = buffer_size;
            if (type == IPVersion.IPv4only)
            {
                server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                local_endpoint = new IPEndPoint(IPAddress.Any, port);
            }else if (type == IPVersion.IPv6only)
            {
                server_socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                local_endpoint = new IPEndPoint(IPAddress.IPv6Any, port);
            }else if (type == IPVersion.IPv4v6)
            {
                server_socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                server_socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
                local_endpoint = new IPEndPoint(IPAddress.IPv6Any, port);
            }
            else
            {
                throw new Exception("Not implemented IPVersion!");
            }
            server_socket.Bind(local_endpoint);            
        }

        public void Listen(int i)
        {
            if (server_socket == null)
            {
                throw new Exception("Server socket can not be NULL!");
            }
            server_socket.Listen(i);

            ServerStarted();
            Socket remote_client = server_socket.Accept();

            remote_client.Receive(receive_buffer);            
            string recv = EncodeBytesToString(receive_buffer);
            IncomingConnection(new RemoteClient(remote_client, recv, DateTime.Now));                       
        }

        public string EncodeBytesToString(byte[] buffer)
        {            
            try
            {
                return Encoding.UTF8.GetString(buffer).Replace("\0", "");
            }
            catch
            {
                return Encoding.UTF8.GetString(buffer);   
            }
        }
    }

    public enum IPVersion
    {
        IPv4only,
        IPv6only,
        IPv4v6
    }
}
