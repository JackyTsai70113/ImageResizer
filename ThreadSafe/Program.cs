using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadSafety {

    /// <summary>
    /// 透過 lock 安全的操作變數
    /// </summary>
    class Program {

        /// <summary>
        /// 計數器
        /// </summary>
        private static int _counter;

        /// <summary>
        /// 最大迴圈數
        /// </summary>
        private static readonly int _maxLoop = 10000000;

        /// <summary>
        /// lock物件
        /// </summary>
        private static object _lockObj = new object();

        public static void Main() {
            Task task1, task2;

            _counter = 0;
            Console.WriteLine($"透過非安全方法創建工作");
            //透過 Task.Run 建立並啟動 將 _counter進行多次加1的Task
            task1 = Task.Run(() => AddCountNotSafe(ref _counter));
            task2 = Task.Run(() => AddCountNotSafe(ref _counter));

            task1.Wait();
            task2.Wait();

            Console.WriteLine($"兩個執行緒聯合計算結果是:{_counter:N0}(正確:{2 * _maxLoop:N0})");
            Console.WriteLine();

            _counter = 0;
            Console.WriteLine($"透過安全方法創建工作");
            task1 = Task.Run(() => AddCountSafe(ref _counter));
            task2 = Task.Run(() => AddCountSafe(ref _counter));

            task1.Wait();
            task2.Wait();

            Console.WriteLine($"兩個執行緒聯合計算結果是:{_counter:N0}(正確:{2 * _maxLoop:N0})");
            Console.WriteLine();

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        /// <summary>
        /// 將 counter 進行多次的 "加1"
        /// </summary>
        /// <param name="counter"></param>
        private static void AddCountNotSafe(ref int counter) {
            for (int index = 0; index < _maxLoop; index++) {
                //未用Lock，多執行緒同時操作時，會使得變數取值和賦值有問題
                int newCounter = counter + 1;
                counter = newCounter;
            }
            Console.WriteLine($"不安全的執行緒{Thread.CurrentThread.ManagedThreadId} 計算完 {_maxLoop:N0} 次");
        }

        /// <summary>
        /// 將 counter 進行多次的 "加1" (使用 lock 確保一次只會有一個執行緒操作)
        /// </summary>
        /// <param name="counter"></param>
        private static void AddCountSafe(ref int counter) {
            for (int index = 0; index < _maxLoop; index++) {
                //透過搶Lock的方式確保一次只會有一個執行緒操作 "加1"
                lock (_lockObj) {
                    int newCounter = counter + 1;
                    counter = newCounter;
                }
            }
            Console.WriteLine($"安全的執行緒{Thread.CurrentThread.ManagedThreadId} 計算完 {_maxLoop:N0} 次");
        }
    }
}