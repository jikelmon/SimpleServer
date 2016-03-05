using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace NetworkingLib.TCP
{
    public class RemoteClient
    {
        public Socket remote_endpoint { get; set; }
        public string received_data { get; set; }
        public DateTime timestamp { get; set; }

        public RemoteClient(Socket remote_endpoint, string data, DateTime timestamp)
        {
            this.remote_endpoint = remote_endpoint;
            this.received_data = data;
            this.timestamp = timestamp;
        }

        public void Answer(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            remote_endpoint.Send(buffer);
            remote_endpoint.Close();
        }
    }
}
