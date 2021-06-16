using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MediaPlayer
{
    /// <summary>
    /// Логика взаимодействия для CommonVersion.xaml
    /// </summary>
    public partial class CommonVersion : Window
    {
        List<Playlist> Playlists;
        int playablePlaylist;
        int showingPlaylist;
        int previousMusicSelectedIndex = 0;
        public CommonVersion()
        {
            #region initialization
            InitializeComponent();
            Playlists = new List<Playlist>();
            playablePlaylist = 0;
            showingPlaylist = 0;
            #endregion

            #region creating and filling list of playlists
            List<string> playlistDirectories = new List<string>();
            playlistDirectories.Add(Helper.musicPath);
            playlistDirectories.AddRange(Directory.GetDirectories(Helper.musicPath));
            foreach (string dir in playlistDirectories)
            {
                var playlist = new Playlist(dir.Equals(Helper.musicPath) ? "Вся музыка" : new DirectoryInfo(dir).Name);
                var musicFiles = Directory.GetFiles(dir, "*.mp3");
                foreach (string musicPath in musicFiles)
                    playlist.Music.Add(new Music(musicPath));
                Playlists.Add(playlist);
            }
            foreach (Playlist p in Playlists)
                AddPlaylist(p.Name);
            MusicPlayer.Source = new Uri(Playlists[0].Music[0].Path);
            #endregion

            //по умолчанию заполняем листбокс плейлистом со всей музыкой
            FillCurrentPlaylist(Playlists, 0);
        }

        #region Mediaplayer's buttons
        void NextMusic()
        {
            previousMusicSelectedIndex++;
            if (previousMusicSelectedIndex == CurrentPlaylist.Items.Count)
                previousMusicSelectedIndex = 0;
            MusicPlayer.Source = new Uri(Playlists[playablePlaylist].Music[previousMusicSelectedIndex].Path);
            MusicPlayer.Play();
            PlayButton.IsChecked = true;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)PlayButton.IsChecked)
                MusicPlayer.Play();
            else MusicPlayer.Pause();
        }
        private void NextButton_Click(object sender, RoutedEventArgs e) => NextMusic();
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            previousMusicSelectedIndex--;
            if (previousMusicSelectedIndex == -1)
                previousMusicSelectedIndex = CurrentPlaylist.Items.Count - 1;
            MusicPlayer.Source = new Uri(Playlists[playablePlaylist].Music[previousMusicSelectedIndex].Path);
            MusicPlayer.Play();
            PlayButton.IsChecked = true;
        }
        #endregion

        #region Playlists' functions
        void AddPlaylist(string playlistName)
        {
            #region creating button in UI
            var buttonGrid = new Grid()
            {
                Style = Application.Current.Resources["InButtonGrid"] as Style
            };
            var namePlaylist = new TextBlock()
            {
                Text = playlistName,
                Margin = new Thickness(10, 110, 10, 0)
            };
            var image = new Image()
            {
                Source = new BitmapImage(new Uri(@"D:\Загрузки\playlist_icon.png")),
                Margin = new Thickness(10, 5, 10, 20)
            };
            buttonGrid.Children.Add(namePlaylist);
            buttonGrid.Children.Add(image);
            var playlistButton = new Button()
            {
                Style = Application.Current.Resources["PlaylistStyle"] as Style,
                Content = buttonGrid
            };
            playlistButton.Click += PlaylistButton_Click;
            PlaylistPanel.Children.Add(playlistButton);
            #endregion
        }
        void FillCurrentPlaylist(List<Playlist> playlists, int i)
        {
            CurrentPlaylist.Items.Clear();
            foreach (Music music in playlists[i].Music)
            {
                var panel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Height = 70,
                    VerticalAlignment = VerticalAlignment.Center
                };
                if (music.AlbumCover.Parent != null)
                {
                    var parent = (Panel)music.AlbumCover.Parent;
                    parent.Children.Remove(music.AlbumCover);
                }
                panel.Children.Add(music.AlbumCover);

                var musicName = new TextBlock
                {
                    Height = 30,
                    FontSize = 22,
                    VerticalAlignment = VerticalAlignment.Center
                };
                musicName.Text = (music.Tag == null || !music.Tag.Title.IsAssigned || !music.Tag.Artists.IsAssigned) ?
                    Path.GetFileNameWithoutExtension(music.Path) : $"{music.Tag.Title} - {music.Tag.Artists}";
                var borderForText = new Border()
                {
                    BorderBrush = null,
                    Child = musicName,
                    Padding = new Thickness(20)
                };
                panel.Children.Add(borderForText);

                CurrentPlaylist.Items.Add(panel);
            }
            showingPlaylist = i;
        }
        private void PlaylistButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            var grid = (sender as Button).Content as Grid;
            var name = (grid.Children[0] as TextBlock).Text;
            int index = Playlists.FindIndex(p => p.Name == name);
            if (index != showingPlaylist)
                FillCurrentPlaylist(Playlists, index);
        }
        private void CurrentPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            playablePlaylist = showingPlaylist;
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
                    MusicPlayer.Source = new Uri(Playlists[playablePlaylist].Music[index].Path);
                    MusicPlayer.Play();
                    PlayButton.IsChecked = true;
                    previousMusicSelectedIndex = index;
                }

                //снимает выделение после нажатия (вообще неплохо было бы просто убрать это самое выделение,
                // чтобы его видно не было и дополнительные переменные вводить)
                (sender as ListBox).SelectedItem = null;
            }
        }
        private void MusicPlayer_MediaEnded(object sender, RoutedEventArgs e) => NextMusic();
        private void CreateNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            DialogCreatePlaylist dialogWindow = new DialogCreatePlaylist();
            if ((bool)dialogWindow.ShowDialog())
            {
                //создаем плейлист
                string name = dialogWindow.PlaylistName;
                AddPlaylist(name);
                Directory.CreateDirectory(Helper.musicPath + "\\" + name);
                Playlists.Add(new Playlist(name));
            }
        }

        #endregion

        #region MusicPlayer's Media Slider

        TimeSpan TotalTime;
        private void MusicPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            TotalTime = MusicPlayer.NaturalDuration.TimeSpan;

            // Create a timer that will update the counters and the time slider
            var timerVideoTime = new DispatcherTimer();
            timerVideoTime.Interval = TimeSpan.FromSeconds(0.01);
            timerVideoTime.Tick += Timer_Tick;
            timerVideoTime.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Check if the movie finished calculate it's total time
            if (MusicPlayer.NaturalDuration.HasTimeSpan)
            {
                if (TotalTime.TotalSeconds > 0)
                {
                    // Updating time slider
                    TimeSlider.Value = MusicPlayer.Position.TotalSeconds /
                                       TotalTime.TotalSeconds;
                }
            }
        }
        private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => ((Slider)sender).SelectionEnd = e.NewValue;

        private void TimeSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (TotalTime.TotalSeconds > 0)
            {
                MusicPlayer.Position = TimeSpan.FromSeconds((double)TimeSlider.Value * TotalTime.TotalSeconds);
            }
        }

        #endregion

    }
}
