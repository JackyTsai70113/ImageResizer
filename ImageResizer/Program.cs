using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ImageResizer {

    internal class Program {

        /// <summary>
        /// 新版程式時間
        /// </summary>
        private static long newSpan = 0;

        /// <summary>
        /// 舊版程式時間
        /// </summary>
        private static long origSpan = 1;

        private static void Main(string[] args) {
            ImageProcess _imageProcess = new ImageProcess();

            // 設定來源資料夾及目標資料夾
            string sourcePath = Path.Combine(Environment.CurrentDirectory, "images");
            string destinationPath = Path.Combine(Environment.CurrentDirectory, "output");

            // 清除目標資料夾
            _imageProcess.Clean(destinationPath);

            ImageProcess.Write("default(origSpan): " + GetExecutionTime(Task.Run(() => {
                _imageProcess.ParallelResizeImages(sourcePath, destinationPath, 2.0);
            })));
            //imageProcess.ParallelResizeImages(sourcePath, destinationPath, 2.0);

            // 新版程式
            ImageProcess.Write($"平行處理縮放圖片...");
            newSpan = GetExecutionTime(Task.Run(() => {
                _imageProcess.ParallelResizeImages(sourcePath, destinationPath, 2.0);
            }));

            ImageProcess.Write("平行處理圖片完畢.");

            // 清除目標資料夾
            _imageProcess.Clean(destinationPath);

            // 原版程式
            ImageProcess.Write($"原版縮放圖片程式...");
            origSpan = GetExecutionTime(Task.Run(() => {
                _imageProcess.ResizeImages(sourcePath, destinationPath, 2.0);
            }));
            ImageProcess.Write("處理圖片完畢.");

            // 計算並印出此次結果
            Console.WriteLine($"New: {newSpan} ms");
            Console.WriteLine($"Orig: {origSpan} ms");
            Console.WriteLine(string.Format("效能提升比例: {0:00.0}%", ((double)origSpan - newSpan) / origSpan * 100));

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            // 1. 程式碼需兼顧可讀性(請適度排版)與執行效率
            // 2. 請紀錄修改前與修改後的效能改善幅度(百分比)
            // 3. 完成後請貼上連結與告知效能提升百分比
            // 4. 效能提升比例公式：(Orig - New) / Orig
        }

        /// <summary>
        /// 取得Task執行時間
        /// </summary>
        /// <param name="task"></param>
        /// <returns>time(ms)</returns>
        private static long GetExecutionTime(Task task) {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            task.Wait();
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }
}