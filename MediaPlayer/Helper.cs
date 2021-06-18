using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer
{
    static class Helper
    {
        public static string path = @"C:\Users\ender\Music\";
        public static string musicPath = Directory.GetCurrentDirectory() + @"\MusicPlaylists.pl";
        public static string accountPath = Directory.GetCurrentDirectory() + @"\Accounts.pl";

        public static TimeSpan StripMilliseconds(this TimeSpan time)
        {
            return new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);
        }


        #region Async serialization
        public static async void SerializePlaylistsAsync(List<Playlist> playlists)
        {
            await Task.Run(() =>
            {
                BinaryFormatter formatter = new BinaryFormatter();
                if (File.Exists(musicPath))
                    using (FileStream fileStream = new FileStream(musicPath, FileMode.Truncate))
                        formatter.Serialize(fileStream, playlists);
                else
                    using (FileStream fileStream = new FileStream(musicPath, FileMode.Create))
                        formatter.Serialize(fileStream, playlists);
            });
        }
        public static async void SerializeAccountsAsync(Account account)
        {
            await Task.Run(() =>
            {
                //добавляем или изменяем данные об аккаунте
                List<Account> accounts = DeserializeAccount();
                int index = accounts.FindIndex(a => a.Login == account.Login);
                if (index != -1)
                {
                    accounts.RemoveAt(index);
                    accounts.Insert(index, account);
                }
                else accounts.Add(account);
                SerializeAccountsAsync(accounts);
            });
        }
        public static async void SerializeAccountsAsync(List<Account> accounts)
        {
            await Task.Run(() =>
            {
                BinaryFormatter formatter = new BinaryFormatter();
                if (File.Exists(accountPath))
                    using (FileStream fileStream = new FileStream(accountPath, FileMode.Truncate))
                        formatter.Serialize(fileStream, accounts);
                else
                    using (FileStream fileStream = new FileStream(accountPath, FileMode.Create))
                        formatter.Serialize(fileStream, accounts);
            });
        }
        #endregion

        #region Deserialization
        public static List<Playlist> DeserializeMusicPlaylists()
        {
            List<Playlist> Playlists = new List<Playlist>();
            Playlists.Add(new Playlist("Музыка"));
            string[] files = Directory.GetFiles(@"C:\Users\ender\Music\", "*.mp3");
            if (files != null && files.Length != 0)
                foreach (string s in files)
                    Playlists[0].Music.Add(new Music(s));
            
            string path = musicPath;
            

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
                if (fileStream.Length != 0)
                    Playlists.AddRange((List<Playlist>)formatter.Deserialize(fileStream));
            return Playlists;
        }
        public static List<Account> DeserializeAccount()
        {
            List<Account> accounts = new List<Account>();
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fileStream = new FileStream(accountPath, FileMode.OpenOrCreate))
                if (fileStream.Length != 0)
                    accounts = (List<Account>)formatter.Deserialize(fileStream);
            return accounts;
        }
        #endregion
    }
}
