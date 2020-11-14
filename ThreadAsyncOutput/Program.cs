using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncThread {

    /// <summary>
    /// 執行緒(Thread)的使用
    /// </summary>
    class Program {

        static void Main(string[] args) {
            PrintXAndDash();

            PrintXAndDashByThread();

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        /// <summary>
        /// 同步處理 PrintX 和 PrintDash
        /// </summary>
        private static void PrintXAndDash() {
            Console.WriteLine("同步處理 PrintX 和 PrintDash");
            PrintX();
            PrintDash();
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 非同步透過執行緒處理 PrintX 和 PrintDash
        /// </summary>
        private static void PrintXAndDashByThread() {
            Console.WriteLine("非同步透過執行緒處理 PrintX 和 PrintDash");
            //建立執行緒
            Thread thread1 = new Thread(() => {
                PrintX();
            });
            Thread thread2 = new Thread(() => {
                PrintDash();
            });

            //啟動執行緒
            thread1.Start();
            thread2.Start();

            //等待執行緒
            thread1.Join();
            thread2.Join();

            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 每隔一段時間印出"X"，連續印20個
        /// </summary>
        private static void PrintX() {
            for (int i = 0; i < 20; i++) {
                Thread.Sleep(10);
                Console.Write("X");
            }
        }

        /// <summary>
        /// 每隔一段時間印出"-"，連續印20個
        /// </summary>
        private static void PrintDash() {
            for (int i = 0; i < 20; i++) {
                Thread.Sleep(15);
                Console.Write("-");
            }
        }
    }
}