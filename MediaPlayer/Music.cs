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
    class Music
    {
        public string Path { get; set; }
        public Image AlbumCover { get; set; }
        public Id3Tag Tag { get; set; }
        Music()
        {

        }
        public Music(string Path)
        {
            this.Path = Path;
            var MP3 = new Mp3(Path);
            Tag = MP3.GetTag(Id3TagFamily.Version2X);
            var audio = new TagLib.Mpeg.AudioFile(Path);
            AlbumCover = new Image
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
    }
}
