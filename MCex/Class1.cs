using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MCex
{
    public class MCSearch
    {
        private static bool req_flag = false;
        private static bool res_flag = false;

        public static async Task<List<string>> MulticastSercher(string mc_address, string yc_address, int reqport, int resport, int timeout = 1000)
        {
            if (req_flag == true)
            {
                return null;
            }

            List<string> list = new();

            //レスポンスのリクエストを送信
            MCRequester.RequestSender(mc_address, yc_address, reqport);

            //非同期の別スレッドでリクエストをListen
            MCRequester.ResponceListener(mc_address, yc_address, resport);


            //udpclientの終了処理
            Thread.Sleep(timeout);
            MCRequester.flag = true;
            MCRequester.udpClient.Close();

            list = MCRequester.reslist;

            Thread.Sleep(100);

            return list;
        }

        public static void MulticastResponser(string mc_address, string yc_address, int reqport, int resport)
        {
            if (res_flag == true)
            {
                return;
            }

            res_flag = true;

            MCResponser.WaitRequest(mc_address, yc_address, reqport);

            while (res_flag == true)
            {
                MCResponser.mre.WaitOne();
                MCResponser.SendResponse(mc_address, yc_address, resport);
                MCResponser.mre.Reset();
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
