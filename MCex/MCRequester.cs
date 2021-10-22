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
        private static List<string> reslist;
        public static bool flag = false;
        
        public static UdpClient udpClient;
        public static ManualResetEvent mre = new ManualResetEvent(false);

        public static List<string> ResponceListener(string mc_address , string yc_address , int resport)
        {
            reslist = new();

            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Any , resport);
            udpClient = new UdpClient(iPEnd);
            udpClient.JoinMulticastGroup(IPAddress.Parse(mc_address), IPAddress.Parse(yc_address));
            udpClient.BeginReceive(ReceiveCallback, udpClient);

            mre.Set();

            return reslist;
        }

        public static void RequestSender(string mc_address , string yc_address , int reqport)
        {
            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Parse(mc_address), reqport);
            UdpClient udpClient = new UdpClient(AddressFamily.InterNetwork);
            udpClient.JoinMulticastGroup(IPAddress.Parse(mc_address), IPAddress.Parse(yc_address));

            byte[] sendBytes = Encoding.UTF8.GetBytes("search_request");

            udpClient.Send(sendBytes, sendBytes.Length, iPEnd);
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
            catch
            {
                if (flag == true)
                {
                    return;
                }
            }

            string rcvMsg = Encoding.UTF8.GetString(rcvBytes);
            if (rcvMsg == "search_responce")
            {
                reslist.Add(remoteEP.Address.ToString());
            }
        }
    }
}
