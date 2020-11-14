using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadAsyncOutput {

    class Program {

        static void Main(string[] args) {
            //建立執行緒
            Thread thread1 = new Thread(() => {
                Thread.Sleep(900);
                for (int i = 0; i < 20; i++) {
                    Thread.Sleep(100);
                    Console.Write("X");
                }
            });
            Thread thread2 = new Thread(() => {
                Thread.Sleep(900);
                for (int i = 0; i < 20; i++) {
                    Thread.Sleep(150);
                    Console.Write("-");
                }
            });

            //啟動執行緒1
            thread1.Start();
            //啟動執行緒1
            thread2.Start();

            //等待執行緒1
            thread1.Join();
            //等待執行緒2
            thread2.Join();

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }
    }
}