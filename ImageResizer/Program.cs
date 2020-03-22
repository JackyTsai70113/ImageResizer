using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer {

    internal class Program {
        private static readonly Stopwatch sw = new Stopwatch();

        private static void Main(string[] args) {
            HandleImage();
            Console.WriteLine($"New(花費時間): {sw.ElapsedMilliseconds} ms");
            Console.WriteLine($"Orig: 6076 ms");
            Console.ReadLine();

            // 1. 程式碼碼需兼顧可讀性(請適度排版)與執行效率
            // 2. 請紀錄修改前與修改後的效能改善幅度(百分比)
            // 3. 完成後請貼上連結與告知效能提升百分比
            // 4. 效能提升比例公式：(Orig - New) / Orig
        }

        private static void HandleImage() {
            string sourcePath = Path.Combine(Environment.CurrentDirectory, "images");
            string destinationPath = Path.Combine(Environment.CurrentDirectory, "output");

            ImageProcess imageProcess = new ImageProcess();
            ImageProcess.Write("[HandleImage] Init ImageProcess.");
            // 修改成非同步
            imageProcess.Clean(destinationPath);
            //imageProcess.CleanAsync(destinationPath).Wait();

            sw.Start();
            imageProcess.ResizeImagesAsync(sourcePath, destinationPath, 2.0).Wait();
            //task.Wait();
            //imageProcess.ResizeImages(sourcePath, destinationPath, 2.0);
            sw.Stop();
            ImageProcess.Write("[HandleImage] Process done.");
        }
    }
}