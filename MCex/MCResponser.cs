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
        //MCSearcherで触りたい変数をpublicで宣言
        public static UdpClient udpClient;
        public static ManualResetEvent mre = new ManualResetEvent(false);
        public static bool flag = false;


        //
        /*リクエストパケット受信用メソッド*/
        //
        public static void WaitRequest(string mc_address, string yc_address, int reqport)
        {
            //各変数の初期化
            flag = false;
            udpClient = null;

            //マルチキャストに参加したudpクライアントを作成する
            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Any, reqport);
            udpClient = new UdpClient(iPEnd);
            udpClient.JoinMulticastGroup(IPAddress.Parse(mc_address), IPAddress.Parse(yc_address));
            //マルチキャストグループの非同期udp受信を開始
            udpClient.BeginReceive(ReceiveCallback, udpClient);
        }
        //
        /**/
        //


        //
        /*非同期udp受信*/
        //
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
                //client.close()されたとき用の処理
                if (flag == true)
                {
                    return;
                }
                Console.WriteLine(ex.Message);
            }

            //受信したパケットが特定の値のときにManualResetEventをSetして処理を流す
            string rcvMsg = Encoding.UTF8.GetString(rcvBytes);
            if (rcvMsg == "search_request")
            {
                mre.Set();
            }

            //再度受信
            udp.BeginReceive(ReceiveCallback, udp);
        }


        //
        /*response送信用メソッド*/
        //
        public static void SendResponse(string mc_address, string yc_address, int resport)
        {
            //マルチキャストに参加したudpclientを作成する
            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Parse(mc_address), resport);
            UdpClient udp = new UdpClient(AddressFamily.InterNetwork);
            udp.JoinMulticastGroup(IPAddress.Parse(mc_address), IPAddress.Parse(yc_address));

            //responseパケットを作成・送信する
            byte[] sendBytes = Encoding.UTF8.GetBytes("search_responce");
            udp.Send(sendBytes, sendBytes.Length, iPEnd);
        }
        //
        /**/
        //
    }
}
