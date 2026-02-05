using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WechatClear.Core;

namespace WechatClear.ViewModels
{
    public class DetailViewModel : ObservableObject, IDisposable
    {
        public DetailViewModel(string path)
        {
            Action_ImagesSelect = new RelayCommand(o =>
            {
                this.IsSelected = !this.IsSelected;
            });
            Action_Delete = new RelayCommand(o =>
            {
                this.IsDeleted = true;
            });
            this.Path = path;
            Process();
        }

        public string Path { get; }
        public string RealPath
        {
            get
            {
                if (File.Exists(Path))
                {
                    return Path;
                }
                else
                {
                    return "Resources/temp.jpg";
                }
            }
        }
        public MultimediaType multimediaType { get; private set; }
        public BitmapImage Bitmap { get; private set; }

        public ICommand Action_ImagesSelect { get; private set; }

        public ICommand Action_Delete { get; private set; }

        public bool IsLoaded
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }
        public bool IsSelected
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }
        public bool IsDeleted
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }
        private void Process()
        {
            string extension = System.IO.Path.GetExtension(Path).ToLower();
            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp" || extension == ".gif")
            {
                this.multimediaType = MultimediaType.Image;
            }
            else if (extension == ".mp4" || extension == ".avi" || extension == ".mov" || extension == ".wmv" || extension == ".mkv")
            {
                this.multimediaType = MultimediaType.Video;
            }
            else if (extension == ".pdf" || extension == ".doc" || extension == ".docx" || extension == ".xls" || extension == ".xlsx" || extension == ".ppt" || extension == ".pptx" || extension == ".txt")
            {
                this.multimediaType = MultimediaType.Document;
            }
            else if (extension == ".dat")
            {
                this.multimediaType = MultimediaType.Dat;
            }
            else
            {
                this.multimediaType = MultimediaType.Other;
            }

            if (this.multimediaType == MultimediaType.Image)
            {
                BitmapImage bitmap = CreateBitmapImageFromBytes(File.ReadAllBytes(this.RealPath));
                this.Bitmap = bitmap;
                this.IsLoaded = true;
            }
            else if (this.multimediaType == MultimediaType.Dat)
            {
                var data = File.ReadAllBytes(this.RealPath);
                var decodedData = Common.WeChatDecode.Decode(data);
                try
                {
                    BitmapImage bitmap = CreateBitmapImageFromBytes(decodedData);
                    this.Bitmap = bitmap;
                    this.IsLoaded = true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// 从字节数组创建BitmapImage（核心方法）
        /// </summary>
        /// <param name="imageBytes">图片字节数组</param>
        /// <returns>内存创建的BitmapImage</returns>
        private static BitmapImage CreateBitmapImageFromBytes(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                throw new ArgumentException("图片字节数组不能为空或长度为0");
            }

            // 关键：使用using包裹MemoryStream，避免内存泄漏
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                BitmapImage bitmap = new BitmapImage();
                // 必须先设置CacheOption为OnLoad，否则流关闭后图片无法加载
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // 核心配置：加载时缓存，流关闭后仍可用
                bitmap.EndInit();
                bitmap.Freeze(); // 可选：冻结对象，提升性能（不可再修改）
                return bitmap;
            }
        }

        void IDisposable.Dispose()
        {
            if (Bitmap == null) return;

            if (Bitmap.StreamSource != null)
            {
                Bitmap.StreamSource.Dispose();
            }

            Bitmap = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
