using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace mctest
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0, j = 0;
            List<string> list;

            Task.Run(() =>
            {
                MCex.MCSearch.MulticastResponser("224.100.100.100", "192.168.11.147", 1000, 1001);
                Thread.Sleep(2100);
            });

            while (i < 100)
            {
                Thread.Sleep(100);
                list = MCex.MCSearch.MulticastSercher("224.100.100.100", "192.168.11.147", 1000, 1001, 10);

                if (list.Count != 0)
                {
                    foreach (var str in list)
                    {
                        Console.WriteLine(str);
                        j++;
                    }
                }
                else
                {
                    Console.WriteLine("no items");
                }
                i++;
            }

            Console.WriteLine("Collect " + j + "/" + i);
        }
    }
}
