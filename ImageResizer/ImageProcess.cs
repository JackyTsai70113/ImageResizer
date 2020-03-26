using System;
using System.Collections.Concurrent;
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
        /// 進行圖片的縮放作業
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
        /// [平行計算] 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        public void ParallelResizeImages(string sourcePath, string destPath, double scale) {
            //Write($"尋找 [{sourcePath}] 中的所有圖片...");
            var allFiles = FindImages(sourcePath);
            //Write($"尋找完畢.");
            //Write($"開始逐一處理圖片...");
            Parallel.ForEach(allFiles, (filePath) => {
                Image imgPhoto = Image.FromFile(filePath);

                int sourceWidth = imgPhoto.Width;
                int sourceHeight = imgPhoto.Height;

                int destionatonWidth = (int)(sourceWidth * scale);
                int destionatonHeight = (int)(sourceHeight * scale);

                #region 30~80ms

                Bitmap bitmap = ProcessBitmap((Bitmap)imgPhoto,
                    sourceWidth, sourceHeight,
                    destionatonWidth, destionatonHeight);

                #endregion 30~80ms

                string imgName = Path.GetFileNameWithoutExtension(filePath);
                string destFile = Path.Combine(destPath, imgName + ".jpg");
                bitmap.Save(destFile, ImageFormat.Jpeg);
            });
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