using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageResizer {

    internal class Program {
        private static readonly Stopwatch newSw = new Stopwatch();

        private static readonly Stopwatch origSw = new Stopwatch();

        private static long newSpan;

        private static long origSpan;

        private static void Main(string[] args) {
            Task.Run(() => NewHandleImage()).Wait();
            Task.Run(() => OrigHandleImage()).Wait();
            Console.WriteLine($"New: {newSpan} ms");
            Console.WriteLine($"Orig: {origSpan} ms");
            Console.WriteLine(string.Format("效能提升比例：{0:0.000}", ((double)origSpan - newSpan) / origSpan));

            Console.ReadLine();

            // 1. 程式碼需兼顧可讀性(請適度排版)與執行效率
            // 2. 請紀錄修改前與修改後的效能改善幅度(百分比)
            // 3. 完成後請貼上連結與告知效能提升百分比
            // 4. 效能提升比例公式：(Orig - New) / Orig
        }

        private static void OrigHandleImage() {
            string sourcePath = Path.Combine(Environment.CurrentDirectory, "images");
            string destinationPath = Path.Combine(Environment.CurrentDirectory, "output");

            ImageProcess imageProcess = new ImageProcess();
            //imageProcess.Clean(destinationPath);

            origSw.Start();
            //task.Wait();
            imageProcess.ResizeImages(sourcePath, destinationPath, 2.0);
            origSw.Stop();
            origSpan = origSw.ElapsedMilliseconds;
            ImageProcess.Write("[HandleImage] Process done.");
        }

        private static void NewHandleImage() {
            Console.WriteLine(
                $"#{Thread.CurrentThread.ManagedThreadId} " +
                $"[{DateTime.Now:HH:mm:ss.fff}] ");
            string sourcePath = Path.Combine(Environment.CurrentDirectory, "images");
            string destinationPath = Path.Combine(Environment.CurrentDirectory, "output");

            ImageProcess imageProcess = new ImageProcess();
            ImageProcess.Write($"清除目標資料夾中資料...");
            imageProcess.Clean(destinationPath);
            ImageProcess.Write($"清除完畢");
            Console.WriteLine(
                $"#{Thread.CurrentThread.ManagedThreadId} " +
                $"[{DateTime.Now:HH:mm:ss.fff}] ");
            newSw.Start();
            imageProcess.ResizeImagesAsync(sourcePath, destinationPath, 2.0).Wait();
            //imageProcess.ResizeImages(sourcePath, destinationPath, 2.0);
            //task.Wait();
            newSw.Stop();
            newSpan = newSw.ElapsedMilliseconds;
            ImageProcess.Write("圖片處理完畢");
        }
    }
}