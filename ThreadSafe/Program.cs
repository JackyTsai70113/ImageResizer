using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadSafe {

    class Program {
        private static int _counter = 0;
        private static int _maxLoop = 10000000;

        /// <summary>
        /// lock物件
        /// </summary>
        private static object _lockObj = new object();

        public static void Main() {
            //Console.WriteLine($"透過非安全方法創建執行緒");
            //Thread thread1 = new Thread(() => AddCountNotSafe(ref _counter));
            //Thread thread2 = new Thread(() => AddCountNotSafe(ref _counter));

            Console.WriteLine($"透過安全方法創建執行緒");
            Thread thread1 = new Thread(() => AddCountSafe(ref _counter));
            Thread thread2 = new Thread(() => AddCountSafe(ref _counter));

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Console.WriteLine();
            Console.WriteLine($"兩個執行緒聯合計算結果是:{_counter:N0}(正確:{2 * _maxLoop:N0})");
            Console.WriteLine();

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        private static void AddCountNotSafe(ref int counter) {
            for (int index = 0; index < _maxLoop; index++) {
                counter++;
            }
            Console.WriteLine($"不安全的執行緒{Thread.CurrentThread.ManagedThreadId} 計算完 {_maxLoop:N0} 次");
        }

        private static void AddCountSafe(ref int counter) {
            for (int index = 0; index < _maxLoop; index++) {
                lock (_lockObj) {
                    counter++;
                }
            }
            Console.WriteLine($"安全的執行緒{Thread.CurrentThread.ManagedThreadId} 計算完 {_maxLoop:N0} 次");
        }
    }
}