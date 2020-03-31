using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ImageResizer {

    public class ImageProcess {

        /// <summary>
        /// 紀錄當下執行序ID及時間
        /// </summary>
        /// <param name="content"></param>
        public static void Write(string content = "") {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine(
                $"#{threadId} " +
                $"[{DateTime.Now:HH:mm:ss.fff}] " +
                $"{content} ");
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
        private Bitmap ProcessBitmap(Bitmap img, int srcWidth, int srcHeight, int newWidth, int newHeight) {
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
        /// 圖片縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        public void ResizeImages(string sourcePath, string destPath, double scale) {
            var allFiles = FindImages(sourcePath);
            foreach (var filePath in allFiles) {

                #region 9ms

                Image imgPhoto = Image.FromFile(filePath);
                string imgName = Path.GetFileNameWithoutExtension(filePath);

                #endregion 9ms

                #region 0ms

                int sourceWidth = imgPhoto.Width;
                int sourceHeight = imgPhoto.Height;

                int destionatonWidth = (int)(sourceWidth * scale);
                int destionatonHeight = (int)(sourceHeight * scale);

                #endregion 0ms

                #region 75~126ms

                Bitmap processedImage = ProcessBitmap((Bitmap)imgPhoto,
                    sourceWidth, sourceHeight,
                    destionatonWidth, destionatonHeight);

                #endregion 75~126ms

                #region 44ms

                string destFile = Path.Combine(destPath, imgName + ".jpg");
                processedImage.Save(destFile, ImageFormat.Jpeg);

                #endregion 44ms
            }
        }

        /// <summary>
        /// 非同步圖片縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        /// <returns>非同步圖片縮放Task</returns>
        public Task ResizeImagesAsync(string sourcePath, string destPath, double scale) {
            return ResizeImagesAsync(sourcePath, destPath, scale, CancellationToken.None);
        }

        /// <summary>
        /// 可取消的非同步圖片縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        /// <param name="token">取消權杖</param>
        /// <returns></returns>
        public Task ResizeImagesAsync(string sourcePath, string destPath, double scale, CancellationToken token) {
            var allFiles = FindImages(sourcePath);
            List<Task> tasks = new List<Task>();
            foreach (var filePath in allFiles) {
                tasks.Add(Task.Run(() => {
                    ResizeImage(filePath, destPath, scale, ref token);
                }));
            }
            Task whenAll = Task.WhenAll(tasks.ToArray());
            while (true) {
                if (token.IsCancellationRequested) {
                    // 工作已取消
                    Clean(destPath);
                    return Task.CompletedTask;
                } else if (whenAll.Status == TaskStatus.RanToCompletion) {
                    //工作已完成
                    return Task.CompletedTask;
                } else {
                    Console.Write(".");
                    Thread.Sleep(500);
                }
            }
        }

        /// <summary>
        /// 單張圖片縮放作業
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="destPath"></param>
        /// <param name="scale"></param>
        /// <param name="token"></param>
        private void ResizeImage(string filePath, string destPath, double scale, ref CancellationToken token) {
            CancellationToken cancellationToken = token;
            Image imgPhoto = Image.FromFile(filePath);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            int destionatonWidth = (int)(sourceWidth * scale);
            int destionatonHeight = (int)(sourceHeight * scale);

            // 若取消工作則直接回傳
            if (cancellationToken.IsCancellationRequested) {
                return;
            }

            Bitmap bitmap = ProcessBitmap((Bitmap)imgPhoto,
                sourceWidth, sourceHeight,
                destionatonWidth, destionatonHeight);

            // 若取消工作則直接回傳
            if (cancellationToken.IsCancellationRequested) {
                return;
            }

            string imgName = Path.GetFileNameWithoutExtension(filePath);
            string destFile = Path.Combine(destPath, imgName + ".jpg");
            bitmap.Save(destFile, ImageFormat.Jpeg);
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