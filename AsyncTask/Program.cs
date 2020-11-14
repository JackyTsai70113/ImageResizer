using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncTask {

    /// <summary>
    /// 工作(Task)的使用
    /// </summary>
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
            PrintXAndDashByTask();

            //GetTwoUrlResponse();
            //GetTwoUrlResponseAsync().Wait();

            Console.WriteLine("Press any key for continuing...");
            Console.ReadKey();
        }

        /// <summary>
        /// 非同步透過 Task 處理 PrintX 和 PrintDash
        /// </summary>
        private static void PrintXAndDashByTask() {
            Console.WriteLine("非同步透過 Task 處理 PrintX 和 PrintDash");
            //建立工作
            Task task1 = new Task(() => {
                PrintX();
            });
            Task task2 = new Task(() => {
                PrintDash();
            });

            //啟動工作
            task1.Start();
            task2.Start();

            //等待工作(Wait)
            task1.Wait();
            task2.Wait();

            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// 透過 Task.Result 取值(同步方法，可直接執行)
        /// </summary>
        private static void GetTwoUrlResponse() {
            PrintWithTime("透過 Task.Result 取值");

            HttpClient client = new HttpClient();

            // 建立並啟動 取得回應字串的task
            Task<string> response2000Task = client.GetStringAsync(URL2000);
            // 建立並啟動 取得回應字串的task(與 responseTask2000 幾乎是同時啟動)
            Task<string> responseTask3000 = client.GetStringAsync(URL3000);

            // 透過 Task.Result 等待 task 的回傳值 (使主執行緒進行等待)
            string response2000 = response2000Task.Result;
            // 印出 回傳值
            PrintWithTime(response2000);

            string response3000 = responseTask3000.Result;
            PrintWithTime(response3000);

            Console.WriteLine();
        }

        /// <summary>
        /// 透過 await Task 取值(非同步 task，需等待)
        /// </summary>
        private static async Task GetTwoUrlResponseAsync() {
            PrintWithTime("透過 await Task 取值");

            HttpClient client = new HttpClient();

            // 建立並啟動 取得回應字串的task
            Task<string> responseTask2000 = client.GetStringAsync(URL2000);
            // 建立並啟動 取得回應字串的task (與 responseTask2000 幾乎是同時啟動)
            Task<string> responseTask3000 = client.GetStringAsync(URL3000);

            // 透過 await Task 等待並接收 task 的回傳值 (不會使主執行緒進行等待)
            string response2000 = await responseTask2000;
            // 主執行緒取得 response2000 的值後印出
            PrintWithTime(response2000);

            string response3000 = await responseTask3000;
            // 主執行緒取得 response3000 的值後印出
            PrintWithTime(response3000);

            Console.WriteLine();
        }

        /// <summary>
        /// 加上時間印出字串
        /// </summary>
        /// <param name="str">字串</param>
        private static void PrintWithTime(string str = "") {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss fff} {str}");
        }

        /// <summary>
        /// 每隔一段時間印出"X"，連續印20個
        /// </summary>
        private static void PrintX() {
            Thread.Sleep(900);
            for (int i = 0; i < 20; i++) {
                Thread.Sleep(10);
                Console.Write("X");
            }
        }

        /// <summary>
        /// 每隔一段時間印出"-"，連續印20個
        /// </summary>
        private static void PrintDash() {
            Thread.Sleep(900);
            for (int i = 0; i < 20; i++) {
                Thread.Sleep(15);
                Console.Write("-");
            }
        }
    }
}