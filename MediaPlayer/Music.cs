using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Id3;
using Id3.v2;
using System.Threading;
using System.IO;
using System.Windows.Controls;
using TagLib;
using System.Windows.Media.Imaging;

namespace MediaPlayer
{
    [Serializable]
    public class Music
    {
        public string Path { get; set; }
        Music()
        {

        }
        public Music(string Path)
        {
            this.Path = Path;   
        }
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
        public Image GetImage()
        {
            var audio = new TagLib.Mpeg.AudioFile(Path);
            Image AlbumCover = new Image
            {
                Source = new BitmapImage(new Uri(@"D:\Загрузки\music.jpg")),
                Height = 65,
                Width = 65
            };
            if (audio.Tag.Pictures.Length >= 1)
            {
                var bin = audio.Tag.Pictures[0].Data.Data;
                var bmi = LoadImage(bin);

                if (bmi != null)
                    AlbumCover.Source = bmi;
            }
            return AlbumCover;
        }
        public Id3Tag GetTag()
        {
            return new Mp3(Path).GetTag(Id3TagFamily.Version2X);
        }
    }
}
