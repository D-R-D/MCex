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
        //MCSearchから触りたい変数をpublicで宣言
        public static List<string> reslist;
        public static bool flag = false;
        public static UdpClient udpClient;
        public static ManualResetEvent mre = new ManualResetEvent(false);


        //
        /*レスポンスの受信用メソッド*/
        //
        public static void ResponceListener(string mc_address, string yc_address, int resport)
        {
            //各変数を初期化する
            reslist = null;
            reslist = new();
            flag = false;
            udpClient = null;

            //マルチキャストに参加したudpclientを作成する
            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Any, resport);
            udpClient = new UdpClient(iPEnd);
            udpClient.JoinMulticastGroup(IPAddress.Parse(mc_address), IPAddress.Parse(yc_address));

            //マルチキャストグループの非同期udp受信を開始する
            udpClient.BeginReceive(ReceiveCallback, udpClient);
        }
        //
        /**/
        //

        
        //
        /*マルチキャストのrequest用ポートに送信*/
        //
        public static void RequestSender(string mc_address, string yc_address, int reqport)
        {
            //マルチキャストに参加したudpclientを作成する
            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Parse(mc_address), reqport);
            UdpClient udp = new UdpClient(AddressFamily.InterNetwork);
            udp.JoinMulticastGroup(IPAddress.Parse(mc_address), IPAddress.Parse(yc_address));

            //リクエストパケットを作成・マルチキャストグループに送信する
            byte[] sendBytes = Encoding.UTF8.GetBytes("search_request");
            udp.Send(sendBytes, sendBytes.Length, iPEnd);
        }
        //
        /**/
        //


        //
        /**/
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

            //受信パケットが特定の値のときにreslistにipアドレスを追加する
            string rcvMsg = Encoding.UTF8.GetString(rcvBytes);
            if (rcvMsg == "search_responce")
            {
                reslist.Add(remoteEP.Address.ToString());
            }

            //再度受信
            udp.BeginReceive(ReceiveCallback, udp);
        }
        //
        /**/
        //
    }
}
