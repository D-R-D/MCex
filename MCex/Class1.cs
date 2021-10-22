using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MCex
{
    public class MCSearch
    {
        private static bool req_flag = false;
        private static bool res_flag = false;

        public static List<string> MulticastSercher(string mc_address , string yc_address , int reqport , int resport , int timeout = 1000)
        {
            if(req_flag == true)
            {
                return null;
            }

            List<string> list = new();

            //レスポンスのリクエストを送信
            MCRequester.RequestSender(mc_address, yc_address, reqport);

            //非同期の別スレッドでリクエストをListen
            Task.Run(() =>
            {
                list = MCRequester.ResponceListener(mc_address, yc_address, resport);
            });


            //udpclientの終了処理
            Thread.Sleep(timeout);
            MCRequester.flag = true;
            MCRequester.udpClient.Close();
            
            //処理が確実に終了するまで待つ
            MCRequester.mre.WaitOne();
            MCRequester.mre.Reset(); //とりあえずリセット、いらないけど気持ち的においておく

            Thread.Sleep(100);

            return list;
        }

        public static void MulticastResponser(string mc_address , string yc_address , int reqport , int resport)
        {
            if(res_flag == true)
            {
                return;
            }

            res_flag = true;

            MCResponser.WaitRequest(mc_address, yc_address, reqport);
            while(res_flag == true)
            {
                MCResponser.mre.WaitOne();
                MCResponser.SendResponse(mc_address , yc_address , resport);
            }

        }

        public static void StopResponser()
        {
            res_flag = false;
            MCResponser.udpClient.Close();
            MCResponser.flag = true;
            MCResponser.mre.Set();
        }

    }
}
