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
        public static string musicPath = @"C:\Users\ender\Music\Playlists.pl";

        //Асинхронная сериализация, чтобы не замораживать приложениие в процессе сохранения изменений
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

        public static List<Playlist> DeserializePlaylists()
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
    }
}
