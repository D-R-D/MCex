using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MCex
{
    class MCRequester
    {
        public static List<string> reslist;
        public static bool flag = false;

        public static UdpClient udpClient;
        public static ManualResetEvent mre = new ManualResetEvent(false);

        public static void ResponceListener(string mc_address, string yc_address, int resport)
        {
            reslist = null;
            reslist = new();
            flag = false;
            udpClient = null;

            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Any, resport);
            udpClient = new UdpClient(iPEnd);
            udpClient.JoinMulticastGroup(IPAddress.Parse(mc_address), IPAddress.Parse(yc_address));
            udpClient.BeginReceive(ReceiveCallback, udpClient);
        }

        public static void RequestSender(string mc_address, string yc_address, int reqport)
        {
            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Parse(mc_address), reqport);
            UdpClient udp = new UdpClient(AddressFamily.InterNetwork);
            udp.JoinMulticastGroup(IPAddress.Parse(mc_address), IPAddress.Parse(yc_address));

            byte[] sendBytes = Encoding.UTF8.GetBytes("search_request");

            udp.Send(sendBytes, sendBytes.Length, iPEnd);
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient udp = (UdpClient)ar.AsyncState;

            IPEndPoint remoteEP = null;
            byte[] rcvBytes = null;
            try
            {
                rcvBytes = udp.EndReceive(ar, ref remoteEP);
            }
            catch (Exception ex)
            {
                if (flag == true)
                {
                    return;
                }
                Console.WriteLine(ex.Message);
            }

            string rcvMsg = Encoding.UTF8.GetString(rcvBytes);

            if (rcvMsg == "search_responce")
            {
                reslist.Add(remoteEP.Address.ToString());
            }

            udp.BeginReceive(ReceiveCallback, udp);
        }
    }
}
