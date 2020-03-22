using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;

namespace ImageResizer {

    public class ImageProcess {

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
                // 0ms
                var allImageFiles = Directory.GetFiles(destPath, "*", SearchOption.AllDirectories);
                // 10ms
                foreach (var item in allImageFiles) {
                    File.Delete(item);
                }
            }
        }

        /// <summary>
        /// [非同步] 清空目的目錄下的所有檔案與目錄
        /// </summary>
        /// <param name="destPath">目錄路徑</param>
        public async Task CleanAsync(string destPath) {
            if (!Directory.Exists(destPath)) {
                Directory.CreateDirectory(destPath);
            } else {
                var allImageFiles = Directory.GetFiles(destPath, "*", SearchOption.AllDirectories);
                //List<Task> tasks = new List<Task>();
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

                #region 6~10ms

                Image imgPhoto = Image.FromFile(filePath);
                string imgName = Path.GetFileNameWithoutExtension(filePath);

                #endregion 6~10ms

                #region 可忽略

                int sourceWidth = imgPhoto.Width;
                int sourceHeight = imgPhoto.Height;

                int destionatonWidth = (int)(sourceWidth * scale);
                int destionatonHeight = (int)(sourceHeight * scale);

                #endregion 可忽略

                #region 30~80ms

                Bitmap processedImage = processBitmap((Bitmap)imgPhoto,
                    sourceWidth, sourceHeight,
                    destionatonWidth, destionatonHeight);

                #endregion 30~80ms

                #region 30~60ms

                string destFile = Path.Combine(destPath, imgName + ".jpg");
                processedImage.Save(destFile, ImageFormat.Jpeg);

                #endregion 30~60ms
            }
        }

        /// <summary>
        /// [非同步] 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        public async Task ResizeImagesAsync(string sourcePath, string destPath, double scale) {
            ImageProcess.Write("[ResizeImagesAsync]");

            var allFiles = FindImages(sourcePath);

            List<Task> tasks = new List<Task>();
            foreach (var filePath in allFiles) {
                tasks.Add(Task.Run(async () => {

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

                    Bitmap processedImage = await ProcessBitmapAsync((Bitmap)imgPhoto,
                        sourceWidth, sourceHeight,
                        destionatonWidth, destionatonHeight);

                    #endregion 30~80ms

                    #region 30~60ms

                    string imgName = Path.GetFileNameWithoutExtension(filePath);
                    string destFile = Path.Combine(destPath, imgName + ".jpg");

                    //string destFile = await GetDestFile(destPath, filePath);
                    processedImage.Save(destFile, ImageFormat.Jpeg);

                    #endregion 30~60ms

                    ImageProcess.Write("[ResizeImagesAsync]");
                }));
            }

            await Task.WhenAll(tasks);
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

        private async Task<string> GetDestFile(string destPath, string filePath) {
            string imgName = Path.GetFileNameWithoutExtension(filePath);
            return Path.Combine(destPath, imgName + ".jpg");
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
        private async Task<Bitmap> ProcessBitmapAsync(Bitmap img, int srcWidth, int srcHeight, int newWidth, int newHeight) {
            Write(" 1 Task<Bitmap>");
            Bitmap resizedbitmap = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(resizedbitmap);
            g.InterpolationMode = InterpolationMode.High;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(img,
                new Rectangle(0, 0, newWidth, newHeight),
                new Rectangle(0, 0, srcWidth, srcHeight),
                GraphicsUnit.Pixel);
            Write(" 2 Task<Bitmap>");
            return resizedbitmap;
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
            Bitmap resizedbitmap = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(resizedbitmap);
            g.InterpolationMode = InterpolationMode.High;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(img,
                new Rectangle(0, 0, newWidth, newHeight),
                new Rectangle(0, 0, srcWidth, srcHeight),
                GraphicsUnit.Pixel);
            return resizedbitmap;
        }
    }
}