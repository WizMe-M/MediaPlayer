using Id3;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
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
        bool LoopTrack;
        bool LoopPlaylist;
        List<Playlist> Playlists;
        int playablePlaylist;
        int showingPlaylist;
        int musicSelectedIndex;
        public CommonVersion()
        {
            #region initialization
            InitializeComponent();
            playablePlaylist = 0;
            showingPlaylist = 0;
            musicSelectedIndex = -1;
            LoopPlaylist = false;
            LoopTrack = false;
            Playlists = Helper.DeserializeMusicPlaylists();
            #endregion

            foreach (Playlist p in Playlists)
                AddPlaylist(p.Name);

            foreach (Music m in Playlists[0].Music)
                AddMusicToPlaylist(m);


        }


        #region Mediaplayer's buttons
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (musicSelectedIndex != -1)
            {
                if ((bool)PlayButton.IsChecked)
                    MediaPlayer.Play();
                else MediaPlayer.Pause();
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (musicSelectedIndex != -1 && Playlists[playablePlaylist].Music.Count != 0)
            {
                musicSelectedIndex++;
                if (LoopPlaylist)
                {
                    if (musicSelectedIndex == Playlists[playablePlaylist].Music.Count)
                        musicSelectedIndex = 0;
                    MediaPlayer.Source = new Uri(Playlists[playablePlaylist].Music[musicSelectedIndex].Path);
                    MediaPlayer.Play();
                    PlayButton.IsChecked = true;
                }
                else
                {
                    if (musicSelectedIndex == Playlists[playablePlaylist].Music.Count)
                    {
                        musicSelectedIndex--;
                        MediaPlayer.Stop();
                        PlayButton.IsChecked = false;
                        MediaPlayer.Source = new Uri(Playlists[playablePlaylist].Music[musicSelectedIndex].Path);
                        MessageBox.Show("Плейлист подошел к концу!");
                    }
                    else
                    {
                        MediaPlayer.Source = new Uri(Playlists[playablePlaylist].Music[musicSelectedIndex].Path);
                        MediaPlayer.Play();
                        PlayButton.IsChecked = true;
                    }
                }
            }
        }
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (musicSelectedIndex != -1 && Playlists[playablePlaylist].Music.Count != 0)
            {
                musicSelectedIndex--;
                if (LoopPlaylist)
                {
                    if (musicSelectedIndex == -1)
                        musicSelectedIndex = Playlists[playablePlaylist].Music.Count - 1;
                    MediaPlayer.Source = new Uri(Playlists[playablePlaylist].Music[musicSelectedIndex].Path);
                    MediaPlayer.Play();
                    PlayButton.IsChecked = true;
                }
                else
                {
                    if (musicSelectedIndex == -1)
                    {
                        musicSelectedIndex++;
                        MediaPlayer.Stop();
                        PlayButton.IsChecked = false;
                        MediaPlayer.Source = new Uri(Playlists[playablePlaylist].Music[0].Path);
                        MessageBox.Show("Плейлист дошел до самого начала!");
                    }
                    else
                    {
                        MediaPlayer.Source = new Uri(Playlists[playablePlaylist].Music[musicSelectedIndex].Path);
                        MediaPlayer.Play();
                        PlayButton.IsChecked = true;
                    }
                }
            }
        }

        private void LoopTrack_Click(object sender, RoutedEventArgs e)
        {
            LoopTrack = !LoopTrack;
            LoopPlaylist = false;
            LoopPlaylistBut.IsChecked = false;
        }
        private void LoopPlaylist_Click(object sender, RoutedEventArgs e)
        {
            LoopPlaylist = !LoopPlaylist;
            LoopTrack = false;
            LoopTrackBut.IsChecked = false;
        }
        #endregion

        #region Playlist Buttons' functions
        void AddPlaylist(string playlistName)
        {
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
            playlistButton.MouseRightButtonUp += PlaylistButton_MouseRightButtonUp;
            PlaylistPanel.Children.Add(playlistButton);
        }
        private void PlaylistButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PlaylistPanel.Children[0] != sender)
            {
                var panel = (sender as Button).Content as Grid;

                int deleteIndex = Playlists.FindIndex(p => p.Name == (panel.Children[0] as TextBlock).Text);
                PlaylistPanel.Children.RemoveAt(deleteIndex);
                Playlists.RemoveAt(deleteIndex);

                if (deleteIndex == playablePlaylist)
                {
                    showingPlaylist = 0;
                    playablePlaylist = 0;
                    musicSelectedIndex = 0;
                    MediaPlayer.Stop();
                    MediaPlayer.Source = null;
                    PlayButton.IsChecked = false;
                }

                if (deleteIndex <= showingPlaylist)
                    showingPlaylist--;
                if (deleteIndex < playablePlaylist)
                    playablePlaylist--;

                FillCurrentPlaylist(playablePlaylist);
                Helper.SerializePlaylistsAsync(Playlists);
            }
            else
            {
                MessageBox.Show("Нельзя удалить главный плейлист со всей музыкой");
            }
        }

        #endregion

        #region Playlist's functions
        void AddMusicToPlaylist(Music music)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Height = 70,
                VerticalAlignment = VerticalAlignment.Center
            };

            Image image = music.GetImage();
            panel.Children.Add(image);

            var musicName = new TextBlock
            {
                Height = 30,
                FontSize = 22,
                VerticalAlignment = VerticalAlignment.Center
            };
            Id3Tag tag = music.GetTag();
            musicName.Text = (tag == null || !tag.Title.IsAssigned || !tag.Artists.IsAssigned) ?
                Path.GetFileNameWithoutExtension(music.Path) : $"{tag.Title} - {tag.Artists}";
            var borderForText = new Border()
            {
                BorderBrush = null,
                Child = musicName,
                Padding = new Thickness(20)
            };
            panel.Children.Add(borderForText);

            var deleteButton = new Button()
            {
                Height = 50,
                Width = 50,
                Content = "X",
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Right,
                Uid = music.Path
            };
            deleteButton.Click += DeleteSongFromPlaylist_Click;
            panel.Children.Add(deleteButton);

            CurrentPlaylist.Items.Add(panel);
        }
        void FillCurrentPlaylist(int i)
        {
            CurrentPlaylist.Items.Clear();
            foreach (Music music in Playlists[i].Music)
                AddMusicToPlaylist(music);
            showingPlaylist = i;
        }
        private void CurrentPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index;
            if ((sender as ListBox).SelectedItem != null)
            {
                index = (sender as ListBox).SelectedIndex;
                if (index == musicSelectedIndex && showingPlaylist == playablePlaylist)
                {
                    if ((bool)PlayButton.IsChecked)
                        MediaPlayer.Pause();
                    else MediaPlayer.Play();
                    PlayButton.IsChecked = !(bool)PlayButton.IsChecked;
                }
                else
                {
                    playablePlaylist = showingPlaylist;
                    MediaPlayer.Source = new Uri(Playlists[playablePlaylist].Music[index].Path);
                    MediaPlayer.Play();
                    PlayButton.IsChecked = true;
                    musicSelectedIndex = index;
                }

                //снимает выделение после нажатия (вообще неплохо было бы просто убрать это самое выделение,
                // чтобы его видно не было и дополнительные переменные вводить)
                (sender as ListBox).SelectedItem = null;
            }
        }
        private void CreateNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            DialogCreatePlaylist dialogWindow = new DialogCreatePlaylist();
            if ((bool)dialogWindow.ShowDialog())
            {
                string name = dialogWindow.PlaylistName;
                AddPlaylist(name);
                Playlists.Add(new Playlist(name));
                Helper.SerializePlaylistsAsync(Playlists);
            }
        }
        private void PlaylistButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            var grid = (sender as Button).Content as Grid;
            var name = (grid.Children[0] as TextBlock).Text;
            int index = Playlists.FindIndex(p => p.Name == name);
            if (index != showingPlaylist)
                FillCurrentPlaylist(index);
        }
        #endregion

        #region Mediaplayer's functions
        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (!LoopTrack)
            {
                musicSelectedIndex++;
                if (LoopPlaylist)
                {
                    if (musicSelectedIndex == Playlists[playablePlaylist].Music.Count)
                        musicSelectedIndex = 0;
                    MediaPlayer.Source = new Uri(Playlists[playablePlaylist].Music[musicSelectedIndex].Path);
                    MediaPlayer.Play();
                    PlayButton.IsChecked = true;
                }
                else
                {
                    if (musicSelectedIndex == Playlists[playablePlaylist].Music.Count)
                    {
                        musicSelectedIndex--;
                        MediaPlayer.Stop();
                        PlayButton.IsChecked = false;
                        MediaPlayer.Source = new Uri(Playlists[playablePlaylist].Music[musicSelectedIndex].Path);
                        MessageBox.Show("Плейлист подошел к концу!");
                    }
                    else
                    {
                        MediaPlayer.Source = new Uri(Playlists[playablePlaylist].Music[musicSelectedIndex].Path);
                        MediaPlayer.Play();
                        PlayButton.IsChecked = true;
                    }
                }
            }
            else
            {
                MediaPlayer.Stop();
                MediaPlayer.Play();
            }
        }
        #endregion

        #region MediaPlayer's Media Slider

        TimeSpan TotalTime;
        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            TotalTime = MediaPlayer.NaturalDuration.TimeSpan;

            // Create a timer that will update the counters and the time slider
            var timerVideoTime = new DispatcherTimer();
            timerVideoTime.Interval = TimeSpan.FromSeconds(0.01);
            timerVideoTime.Tick += Timer_Tick;
            timerVideoTime.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Check if the movie finished calculate it's total time
            if (MediaPlayer.NaturalDuration.HasTimeSpan)
            {
                if (TotalTime.TotalSeconds > 0)
                {
                    // Updating time slider
                    TimeSlider.Value = MediaPlayer.Position.TotalSeconds /
                                       TotalTime.TotalSeconds;
                }
            }
        }
        private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => ((Slider)sender).SelectionEnd = e.NewValue;

        private void TimeSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (MediaPlayer.Source != null)
            {
                if (TotalTime.TotalSeconds > 0)
                {
                    MediaPlayer.Position = TimeSpan.FromSeconds((double)TimeSlider.Value * TotalTime.TotalSeconds);
                }
            }
        }

        #endregion

        private void AddSongToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (Playlists.Count != 0)
            {
                OpenFileDialog fileDialog = new OpenFileDialog
                {
                    Filter = "MP3 files (*.mp3)|*.mp3"
                };
                if ((bool)fileDialog.ShowDialog())
                {
                    Music music = new Music(fileDialog.FileName);
                    Playlists[showingPlaylist].Music.Add(music);
                    AddMusicToPlaylist(music);
                    Helper.SerializePlaylistsAsync(Playlists);
                }
            }
        }
        private void DeleteSongFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            int index = Playlists[showingPlaylist].Music.FindIndex(m => m.Path == (sender as Button).Uid);
            Playlists[showingPlaylist].Music.RemoveAt(index);
            CurrentPlaylist.Items.RemoveAt(index);
            if (index == musicSelectedIndex && showingPlaylist == playablePlaylist)
            {
                MediaPlayer.Stop();
                PlayButton.IsChecked = false;
                if (Playlists[showingPlaylist].Music.Count == 0)
                {
                    musicSelectedIndex = -1;
                    MediaPlayer.Source = null;
                }
                else
                {
                    MediaPlayer.Source = new Uri(Playlists[showingPlaylist].Music[musicSelectedIndex].Path);
                }
            }
            Helper.SerializePlaylistsAsync(Playlists);
            MessageBox.Show("Песня была удалена из плейлиста");
        }

        private void RefreshPlaylist_Click(object sender, RoutedEventArgs e)
        {
            Playlists[0].Music.Clear();
            string[] files = Directory.GetFiles(@"C:\Users\ender\Music\", "*.mp3");
            if (files != null && files.Length != 0)
                foreach (string s in files)
                    Playlists[0].Music.Add(new Music(s));
        }
    }
}
