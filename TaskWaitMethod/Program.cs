using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TaskWaitMethod {

    class Program {

        /// <summary>
        /// 等待2秒後，回傳"Delay 2000ms"的網址
        /// </summary>
        private static readonly string URL2000 = "http://mocky.azurewebsites.net/api/delay/2000";

        /// <summary>
        /// 等待3秒後，回傳"Delay 3000ms"的網址
        /// </summary>
        private static readonly string URL3000 = "http://mocky.azurewebsites.net/api/delay/3000";

        static void Main(string[] args) {
            NoWaitTask();
            //WaitTask();
            //WaitAllTasks();
            //WaitAnyTasks();
            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        /// <summary>
        /// 建立並啟動 Task，未等候，主執行緒繼續執行
        /// </summary>
        private static void NoWaitTask() {
            PrintWithTime("NoWaitTask Start");

            HttpClient client = new HttpClient();

            //建立並啟動 Task
            Task.Run(() => {
                //建立並啟動 Task<string> (等待2秒後，會回傳"Delay 2000ms")
                Task<string> response2000Task = client.GetStringAsync(URL2000);
                // 透過 Task.Result 等待 task 的回傳值
                string response = response2000Task.Result;
                // 印出回傳的字串
                PrintWithTime("NoWaitTask " + response);
            });
        }

        /// <summary>
        /// 用 Task.Wait() 讓主執行緒暫停執行，等候單一 Task 完成執行
        /// </summary>
        private static void WaitTask() {
            PrintWithTime("WaitTask Start");

            HttpClient client = new HttpClient();

            Task task = Task.Run(() => {
                string response = client.GetStringAsync(URL2000).Result;
                PrintWithTime("WaitTask " + response);
            });
            //主執行緒暫停並等候單一 Task 完成執行
            task.Wait();
        }

        /// <summary>
        /// 用 Task.WaitAll(Task[]) 讓主執行緒暫停執行，等候所有 Task 完成執行
        /// </summary>
        private static void WaitAllTasks() {
            PrintWithTime("WaitAllTasks Start");

            HttpClient client = new HttpClient();

            Task task = Task.Run(() => {
                string response = client.GetStringAsync(URL2000).Result;
                PrintWithTime("WaitAllTasks " + response);
            });
            Task task2 = Task.Run(() => {
                string response = client.GetStringAsync(URL3000).Result;
                PrintWithTime("WaitAllTasks " + response);
            });

            List<Task> tasks = new List<Task>();
            tasks.Add(task);
            tasks.Add(task2);
            //主執行緒暫停並等候所有 Task 完成執行
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// 用 Task.WaitAny(Task[]) 讓主執行緒暫停執行，等候任一 Task 完成執行
        /// </summary>
        private static void WaitAnyTasks() {
            PrintWithTime("WaitAnyTasks Start");

            HttpClient client = new HttpClient();

            Task task = Task.Run(() => {
                string response = client.GetStringAsync(URL2000).Result;
                PrintWithTime("WaitAnyTasks " + response);
            });
            Task task2 = Task.Run(() => {
                string response = client.GetStringAsync(URL3000).Result;
                PrintWithTime("WaitAnyTasks " + response);
            });

            List<Task> tasks = new List<Task>();
            tasks.Add(task);
            tasks.Add(task2);
            //主執行緒暫停並等候任一 Task 完成執行
            Task.WaitAny(tasks.ToArray());
        }

        /// <summary>
        /// 加上時間印出字串
        /// </summary>
        /// <param name="str">字串</param>
        private static void PrintWithTime(string str = "") {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss fff} {str}");
        }
    }
}