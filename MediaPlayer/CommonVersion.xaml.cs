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
        int choosenPlaylist;
        int previousMusicSelectedIndex = 0;
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

            //по умолчнию заполняем листбокс плейлистом со всей музыкой
            FillCurrentPlaylist(Playlists, 0);
        }

        #region Mediaplayer's buttons
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)PlayButton.IsChecked)
                MusicPlayer.Play();
            else MusicPlayer.Pause();
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            previousMusicSelectedIndex++;
            if (previousMusicSelectedIndex == CurrentPlaylist.Items.Count)
                previousMusicSelectedIndex = 0;
            MusicPlayer.Source = new System.Uri(Playlists[choosenPlaylist].Music[previousMusicSelectedIndex].Path);
            MusicPlayer.Play();
            PlayButton.IsChecked = true;
        }
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            previousMusicSelectedIndex--;
            if (previousMusicSelectedIndex == -1)
                previousMusicSelectedIndex = CurrentPlaylist.Items.Count - 1;
            MusicPlayer.Source = new System.Uri(Playlists[choosenPlaylist].Music[previousMusicSelectedIndex].Path);
            MusicPlayer.Play();
            PlayButton.IsChecked = true;
        }
        #endregion

        #region Playlists' functions
        void FillCurrentPlaylist(List<Playlist> playlists, int i)
        {
            CurrentPlaylist.Tag = playlists[i].Name;
            foreach (Music music in playlists[i].Music)
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
            choosenPlaylist = i;
        }

        private void PlaylistButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            var panel = (sender as Button).Content as StackPanel;
            var name = (panel.Children[1] as TextBlock).Text;
            int index = Playlists.FindIndex(p => p.Name == name);
            FillCurrentPlaylist(Playlists, index);
        }
        private void CurrentPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index;
            if ((sender as ListBox).SelectedItem != null)
            {
                index = (sender as ListBox).SelectedIndex;
                if (index == previousMusicSelectedIndex)
                {
                    if ((bool)PlayButton.IsChecked)
                        MusicPlayer.Pause();
                    else MusicPlayer.Play();
                    PlayButton.IsChecked = !(bool)PlayButton.IsChecked;
                }
                else
                {
                    MusicPlayer.Source = new System.Uri(Playlists[choosenPlaylist].Music[index].Path);
                    MusicPlayer.Play();
                    PlayButton.IsChecked = true;
                    previousMusicSelectedIndex = index;
                }
                //снимает выделение после нажатия
                (sender as ListBox).SelectedItem = null;
            }


        }


        #endregion

    }
}
