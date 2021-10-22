using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCex
{
    class MCResponser
    {
        public static UdpClient udpClient;
        public static ManualResetEvent mre = new ManualResetEvent(false);
        public static bool flag = false;


        public static void WaitRequest(string mc_address, string yc_address, int reqport)
        {
            flag = false;
            udpClient = null;

            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Any, reqport);
            udpClient = new UdpClient(iPEnd);
            udpClient.JoinMulticastGroup(IPAddress.Parse(mc_address), IPAddress.Parse(yc_address));
            udpClient.BeginReceive(ReceiveCallback, udpClient);
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
            if (rcvMsg == "search_request")
            {
                mre.Set();
            }

            udp.BeginReceive(ReceiveCallback, udp);
        }

        public static void SendResponse(string mc_address, string yc_address, int resport)
        {
            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Parse(mc_address), resport);
            UdpClient udp = new UdpClient(AddressFamily.InterNetwork);
            udp.JoinMulticastGroup(IPAddress.Parse(mc_address), IPAddress.Parse(yc_address));

            byte[] sendBytes = Encoding.UTF8.GetBytes("search_responce");

            udp.Send(sendBytes, sendBytes.Length, iPEnd);

            mre.Reset();
        }
    }
}
