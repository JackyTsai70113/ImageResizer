using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireAndForgetTask {

    /// <summary>
    /// 1. 啟動 會引發的 Task，卻未等候
    /// 2. 透過 Task 的內建方法取得 Task 目前執行的狀態
    /// </summary>
    class Program {

        static void Main(string[] args) {
            try {
                Task errorTask = ThrowErrorTaskAsync();

                PrintTaskInfo(errorTask);

                Console.WriteLine("按下任一鍵，確認 Task 狀態...(時間短不會發現錯誤，時間長會發現錯誤)");
                Console.ReadKey();

                Console.WriteLine();
                Console.WriteLine();

                PrintTaskInfo(errorTask);

                Console.WriteLine("程式已結束...");
                //ThrowErrorTaskAsync().Wait();
            } catch (Exception ex) {
                Console.WriteLine("錯誤訊息: " + ex.InnerException.Message);
                Console.WriteLine("程式有錯誤...");
            }
            Console.ReadKey();
        }

        /// <summary>
        /// 取得會引發錯誤的 Task
        /// </summary>
        /// <returns>Task: 執行3秒之後會引發錯誤</returns>
        private static async Task ThrowErrorTaskAsync() {
            Console.WriteLine("task 開始執行了");
            Console.WriteLine();
            await Task.Delay(3000);
            throw new Exception("發生錯誤了");
        }

        /// <summary>
        /// Task 本身有方法可以確認目前執行狀態，甚至是錯誤訊息
        /// </summary>
        /// <param name="task">欲確認狀態的Task</param>
        private static void PrintTaskInfo(Task task) {
            Console.WriteLine("Task 狀態:");
            //工作狀態(未啟動/有錯誤/已取消/已完成/等待中...)
            Console.WriteLine($"Status      : {task.Status}");
            //工作是否已完成(有錯誤/已取消/已完成 都算)
            Console.WriteLine($"IsCompleted : {task.IsCompleted}");
            //工作是否已取消
            Console.WriteLine($"IsCanceled  : {task.IsCanceled}");
            //工作是否有錯誤
            Console.WriteLine($"IsFaulted   : {task.IsFaulted}");
            //透過 Task.Exception 可以確認Task 目前有沒有Exception
            var exceptionStatus = task.Exception?.InnerExceptions.Count > 0 ?
                $"有 Exception 物件(訊息: {task.Exception?.InnerException.Message})" : "沒有 Exception 物件";
            Console.WriteLine($"Exception   : {exceptionStatus}");
            Console.WriteLine();
        }
    }
}