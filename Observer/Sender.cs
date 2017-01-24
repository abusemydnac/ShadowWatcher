﻿using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ShadowWatcher
{
    public static class Sender
    {
        private readonly static IPEndPoint dstIP = new IPEndPoint(IPAddress.Loopback, 37954);
        private readonly static UdpClient client = new UdpClient();

        public static void Send(string str)
        {
            var buf = Encoding.UTF8.GetBytes(str);
            client.Send(buf, buf.Length, dstIP);
        }
    }
}
