using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MediaPlayer
{
    /// <summary>
    /// Логика взаимодействия для CommonVersion.xaml
    /// </summary>
    public partial class CommonVersion : Window
    {
        List<Playlist> Playlists;
        FileSystemWatcher MusicWatcher;

        public CommonVersion()
        {
            #region initialization
            InitializeComponent();
            Playlists = new List<Playlist>();
            MusicWatcher = new FileSystemWatcher()
            {
                Path = @"C:\Users\ender\Music\",
                Filter = "*.mp3",
                IncludeSubdirectories = false,
                InternalBufferSize = 32768,
                NotifyFilter = NotifyFilters.LastWrite
            | NotifyFilters.LastAccess
            | NotifyFilters.CreationTime
            | NotifyFilters.FileName
            | NotifyFilters.Attributes
            | NotifyFilters.Size
            };
            #endregion

            #region creating and filling list of playlists
            List<string> playlistDirectories = new List<string>();
            playlistDirectories.Add(Helper.musicPath);
            playlistDirectories.AddRange(Directory.GetDirectories(Helper.musicPath));
            foreach (string dir in playlistDirectories)
            {
                var playlist = new Playlist(dir.Equals(Helper.musicPath) ? "Вся музыка" : Path.GetDirectoryName(dir));
                var musicFiles = Directory.GetFiles(Helper.musicPath, "*.mp3");
                foreach (string musicPath in musicFiles)
                    playlist.Music.Add(new Music(musicPath));
                Playlists.Add(playlist);
            }
            #endregion


            FillCurrentPlaylist(Playlists[0]);
        }

        #region Mediaplayer's buttons
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MusicPlayer.LoadedBehavior = (bool)PlayButton.IsChecked ? MediaState.Play : MediaState.Pause;
        }
        #endregion

        #region Playlists' functions
        private void FillCurrentPlaylist(Playlist playlist)
        {
            CurrentPlaylist.Tag = playlist.Name;
            foreach (Music music in playlist.Music)
            {
                var panel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Height = 70,
                    VerticalAlignment = VerticalAlignment.Center
                };
                panel.Children.Add(music.AlbumCover);

                var musicName = new TextBlock
                {
                    Height = 30,
                    FontSize = 22,
                    VerticalAlignment = VerticalAlignment.Center
                };
                if (music.Tag == null || !music.Tag.Title.IsAssigned || !music.Tag.Artists.IsAssigned)
                    musicName.Text = Path.GetFileNameWithoutExtension(music.Path);
                else musicName.Text = $"{music.Tag.Title} - {music.Tag.Artists}";
                var borderForText = new Border()
                {
                    BorderBrush = null,
                    Child = musicName,
                    Padding = new Thickness(20)
                };
                panel.Children.Add(borderForText);

                CurrentPlaylist.Items.Add(panel);
            }
        }
        #endregion
    }
}
