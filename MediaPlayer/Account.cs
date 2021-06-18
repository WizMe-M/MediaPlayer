using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MediaPlayer
{
    [Serializable]
    public class Account
    {
        public BitmapImage GetIcon()
        {
            return new BitmapImage(new Uri(IconUri));            
        }
        public string IconUri { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<Playlist> Playlists { get; set; }
        public Account(string Login, string Password)
        {
            IconUri = @"D:\Загрузки\basic_icon.svg.png";
            this.Login = Login;
            this.Password = Password;
            Playlists = new List<Playlist>()
            {
                new Playlist("Все медиафайлы")
            };

            string[] files = Directory.GetFiles(Helper.path);
            if (files.Length != 0)
                foreach (string s in files)
                    if (Path.GetExtension(s) == ".mp3" || Path.GetExtension(s) == ".mp4")
                        Playlists[0].Medias.Add(new Media(s));
        }

        public static Account IsExist(string Login, string Password)
        {
            List<Account> accounts = Helper.DeserializeAccount();
            return accounts.Find(a => a.Login == Login && a.Password == Password);
        }
        public static bool IsExist(string Login)
        {
            List<Account> accounts = Helper.DeserializeAccount();
            Account some = accounts.Find(a => a.Login == Login);
            return some != null;
        }
    }
}
