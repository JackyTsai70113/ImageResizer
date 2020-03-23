using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;

namespace ImageResizer {

    public class ImageProcess {
        private static object lockObj = new object();
        private static List<int> tsl2 = new List<int>();
        private static List<int> tsl3 = new List<int>();
        private static List<int> tsl4 = new List<int>();
        private static List<int> tsl5 = new List<int>();
        private static List<int> tsl6 = new List<int>();

        private Bitmap ProcessImage(string filePath, double scale) {

            #region 6~10ms

            Image imgPhoto = Image.FromFile(filePath);

            #endregion 6~10ms

            #region 可忽略

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            int destionatonWidth = (int)(sourceWidth * scale);
            int destionatonHeight = (int)(sourceHeight * scale);

            #endregion 可忽略

            #region 30~80ms

            return processBitmap((Bitmap)imgPhoto,
                 sourceWidth, sourceHeight,
                 destionatonWidth, destionatonHeight);

            #endregion 30~80ms
        }

        private Task<Bitmap> ProcessImageAsync(string filePath, double scale) {
            //Write($"[ProcessImageAsync] 設定圖片縮放格式參數 [{filePath}]...");

            #region 6~10ms

            Image imgPhoto = Image.FromFile(filePath);

            #endregion 6~10ms

            #region 可忽略

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            int destionatonWidth = (int)(sourceWidth * scale);
            int destionatonHeight = (int)(sourceHeight * scale);

            #endregion 可忽略

            #region 30~80ms

            //Write($"[ProcessImageAsync] 設定完畢 [{filePath}]");
            //var a = processBitmap((Bitmap)imgPhoto,
            //    sourceWidth, sourceHeight,
            //    destionatonWidth, destionatonHeight);
            return Task.Run(() => NewProcessBitmap((Bitmap)imgPhoto,
                sourceWidth, sourceHeight,
                destionatonWidth, destionatonHeight));

            #endregion 30~80ms
        }

        /// <summary>
        /// [非同步] 針對指定圖片進行縮放作業
        /// </summary>
        /// <param name="img">圖片來源</param>
        /// <param name="srcWidth">原始寬度</param>
        /// <param name="srcHeight">原始高度</param>
        /// <param name="newWidth">新圖片的寬度</param>
        /// <param name="newHeight">新圖片的高度</param>
        /// <returns></returns>
        private Bitmap NewProcessBitmap(Bitmap img, int srcWidth, int srcHeight, int newWidth, int newHeight) {
            Bitmap resizedbitmap = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(resizedbitmap);
            g.Clear(Color.Transparent); //久
            g.DrawImage(img,
                new Rectangle(0, 0, newWidth, newHeight),
                new Rectangle(0, 0, srcWidth, srcHeight),
                GraphicsUnit.Pixel); //久
            g.InterpolationMode = InterpolationMode.High;
            g.SmoothingMode = SmoothingMode.HighQuality;

            //clearTask.Wait();
            return resizedbitmap;
            //Write(" 1 Task<Bitmap>" + img.ToString());
            //Bitmap resizedbitmap = new Bitmap(newWidth, newHeight);
            //Graphics g = Graphics.FromImage(resizedbitmap);
            //Write(" 2 Task<Bitmap>" + img.ToString());
            //g.InterpolationMode = InterpolationMode.High;
            //g.SmoothingMode = SmoothingMode.HighQuality;
            //g.Clear(Color.Transparent);
            //Write(" 3 Task<Bitmap>" + img.ToString());
            //g.DrawImage(img,
            //    new Rectangle(0, 0, newWidth, newHeight),
            //    new Rectangle(0, 0, srcWidth, srcHeight),
            //    GraphicsUnit.Pixel);
            //Write(" 4 Task<Bitmap>" + img.ToString());
            //return new Task<Bitmap>(resizedbitmap);
        }

        /// <summary>
        /// 針對指定圖片進行縮放作業
        /// </summary>
        /// <param name="img">圖片來源</param>
        /// <param name="srcWidth">原始寬度</param>
        /// <param name="srcHeight">原始高度</param>
        /// <param name="newWidth">新圖片的寬度</param>
        /// <param name="newHeight">新圖片的高度</param>
        /// <returns></returns>
        private Bitmap processBitmap(Bitmap img, int srcWidth, int srcHeight, int newWidth, int newHeight) {
            Write($"1 " + img.GetHashCode());
            DateTime t1 = DateTime.Now;
            Bitmap resizedbitmap = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(resizedbitmap);
            DateTime t2 = DateTime.Now;
            int ts2 = (DateTime.Now - t1).Milliseconds;
            Write($"2 " + img.GetHashCode());
            g.InterpolationMode = InterpolationMode.High;
            DateTime t3 = DateTime.Now;
            int ts3 = (DateTime.Now - t2).Milliseconds;
            Write($"3 " + img.GetHashCode());
            g.SmoothingMode = SmoothingMode.HighQuality;
            DateTime t4 = DateTime.Now;
            int ts4 = (DateTime.Now - t3).Milliseconds;
            Write($"4 " + img.GetHashCode());
            g.Clear(Color.Transparent);
            DateTime t5 = DateTime.Now;
            int ts5 = (DateTime.Now - t4).Milliseconds;
            Write($"5 " + img.GetHashCode());
            g.DrawImage(img,
                new Rectangle(0, 0, newWidth, newHeight),
                new Rectangle(0, 0, srcWidth, srcHeight),
                GraphicsUnit.Pixel);
            DateTime t6 = DateTime.Now;
            int ts6 = (DateTime.Now - t5).Milliseconds;
            Write($"6 " + img.GetHashCode());
            tsl2.Add(ts2);
            tsl3.Add(ts3);
            tsl4.Add(ts4);
            tsl5.Add(ts5);
            tsl6.Add(ts6);
            return resizedbitmap;
        }

        /// <summary>
        /// 紀錄當下執行序ID及時間
        /// </summary>
        /// <param name="content"></param>
        public static void Write(string content = "") {
            Console.WriteLine(
                $"#{Thread.CurrentThread.ManagedThreadId} " +
                $"[{DateTime.Now:HH:mm:ss.fff}] " +
                $"{content} ");
        }

        /// <summary>
        /// 清空目的目錄下的所有檔案與目錄
        /// </summary>
        /// <param name="destPath">目錄路徑</param>
        public void Clean(string destPath) {
            if (!Directory.Exists(destPath)) {
                Directory.CreateDirectory(destPath);
            } else {
                var allImageFiles = Directory.GetFiles(destPath, "*", SearchOption.AllDirectories);
                foreach (var item in allImageFiles) {
                    File.Delete(item);
                }
            }
        }

        /// <summary>
        /// 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        public void ResizeImages(string sourcePath, string destPath, double scale) {
            var allFiles = FindImages(sourcePath);
            foreach (var filePath in allFiles) {
                //Write($"1 [{filePath}]");

                #region 9ms

                Image imgPhoto = Image.FromFile(filePath);
                string imgName = Path.GetFileNameWithoutExtension(filePath);

                #endregion 9ms

                //Write($"2 [{filePath}]");

                #region 0ms

                int sourceWidth = imgPhoto.Width;
                int sourceHeight = imgPhoto.Height;

                int destionatonWidth = (int)(sourceWidth * scale);
                int destionatonHeight = (int)(sourceHeight * scale);

                #endregion 0ms

                //Write($"3 [{filePath}]");

                #region 75~126ms

                Bitmap processedImage = processBitmap((Bitmap)imgPhoto,
                    sourceWidth, sourceHeight,
                    destionatonWidth, destionatonHeight);

                #endregion 75~126ms

                //Write($"4 [{filePath}]");

                #region 44ms

                string destFile = Path.Combine(destPath, imgName + ".jpg");
                processedImage.Save(destFile, ImageFormat.Jpeg);

                #endregion 44ms

                //Write($"5 [{filePath}]");
            }
        }

        /// <summary>
        /// [非同步] 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        public Task ResizeImagesAsync(string sourcePath, string destPath, double scale) {
            Write($"尋找 [{sourcePath}] 中的所有圖片...");
            var allFiles = FindImages(sourcePath);
            Write($"尋找完畢.");
            List<Task> tasks = new List<Task>();
            Write($"開始逐一處理圖片...");
            foreach (var filePath in allFiles) {
                //Write($"開始處理圖片 [{filePath}]");
                Task<Bitmap> ProcessImageAsyncTask = ProcessImageAsync(filePath, scale);

                string imgName = Path.GetFileNameWithoutExtension(filePath);
                string destFile = Path.Combine(destPath, imgName + ".jpg");

                //Task SaveImageTask = ProcessImageAsyncTask.ContinueWith((antecedent) =>
                tasks.Add(ProcessImageAsyncTask.ContinueWith(t => {
                    t.Result.Save(destFile, ImageFormat.Jpeg);
                    Write($"已儲存 [{filePath}]");
                }));
            }
            Task.WhenAll(tasks).Wait();
            Write("After Task.WhenAll(tasks).Wait();");
            /*
            Write(tsl2.Average().ToString());
            Write(tsl2.Max().ToString());
            Write(tsl2.Min().ToString());
            Console.WriteLine();
            Write(tsl3.Average().ToString());
            Write(tsl3.Max().ToString());
            Write(tsl3.Min().ToString());
            Console.WriteLine();
            Write(tsl4.Average().ToString());
            Write(tsl4.Max().ToString());
            Write(tsl4.Min().ToString());
            Console.WriteLine();
            Write(tsl5.Average().ToString());
            Write(tsl5.Max().ToString());
            Write(tsl5.Min().ToString());
            Console.WriteLine();
            Write(tsl6.Average().ToString());
            Write(tsl6.Max().ToString());
            Write(tsl6.Min().ToString());
            Console.WriteLine();
            */
            Task.WhenAll(tasks).Wait();
            return Task.CompletedTask;
        }

        public Task TaskSaveProcessedImageAsync(Bitmap processedImage, string destFile) {
            return Task.Run(() => processedImage.Save(destFile, ImageFormat.Jpeg));
        }

        /// <summary>
        /// 找出指定目錄下的圖片
        /// </summary>
        /// <param name="srcPath">圖片來源目錄路徑</param>
        /// <returns></returns>
        public List<string> FindImages(string srcPath) {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(srcPath, "*.png", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpg", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpeg", SearchOption.AllDirectories));
            return files;
        }
    }
}