using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WechatClear.Core;

namespace WechatClear.ViewModels
{
    public class DetailViewModel : ObservableObject
    {
        public DetailViewModel(string path)
        {
            this.Path = path;
            Process();
        }

        public string Path { get; }
        public MultimediaType multimediaType { get; private set; }
        public BitmapImage Bitmap { get; private set; }
        public bool IsLoaded
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
            if (File.Exists(Path))
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
                else
                {
                    this.multimediaType = MultimediaType.Other;
                }


                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(Path);
                bitmap.EndInit();
                this.Bitmap = bitmap;
                this.IsLoaded = true;
            }
            else
            {
                this.Bitmap = null;
                this.IsLoaded = false;
            }
        }
    }
}
