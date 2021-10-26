using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MCex
{
    public class MCSearch
    {
        //req,res 用のフラッグ
        private static bool req_flag = false;
        private static bool res_flag = false;


        //
        /*マルチキャスト探査を行うメソッド*/
        //
        public static async Task<List<string>> MulticastSercher(string mc_address, string yc_address, int reqport, int resport, int timeout = 1000)
        {
            //フラッグで起動・停止の判別を行う
            //flag == true  →  起動中、nullをreturn
            if (req_flag == true)
            {
                return null;
            }

            //空のlistを作成
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
        //
        /**/
        //


        //
        /*リクエストに対してレスポンスを送信するためのメソッド*/
        //
        public static void MulticastResponser(string mc_address, string yc_address, int reqport, int resport)
        {
            //フラッグで起動・停止の判別を行う
            if (res_flag == true)
            {
                return;
            }
            res_flag = true;

            //requestのlistenを開始
            MCResponser.WaitRequest(mc_address, yc_address, reqport);

            //requestを受信したときresponceを送信する
            while (res_flag == true)
            {
                MCResponser.mre.WaitOne();
                MCResponser.SendResponse(mc_address, yc_address, resport);
                MCResponser.mre.Reset();
            }

        }
        //
        /**/
        //


        //
        /*responserを停止するメソッド*/
        //
        public static void StopResponser()
        {
            res_flag = false;
            MCResponser.udpClient.Close();
            MCResponser.flag = true;
            MCResponser.mre.Set();
        }
        //
        /**/
        //

    }
}
