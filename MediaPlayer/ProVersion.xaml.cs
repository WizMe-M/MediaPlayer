using Id3;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MediaPlayer
{
    /// <summary>
    /// Логика взаимодействия для ProVersion.xaml
    /// </summary>
    public partial class ProVersion : Window
    {
        Account loggedAccount;
        bool LoopTrack;
        bool LoopPlaylist;
        int playablePlaylist;
        int showingPlaylist;
        int mediaSelectedIndex;
        public ProVersion(Account account)
        {
            #region initialization

            InitializeComponent();
            loggedAccount = account;

            UserIcon.ImageSource = new BitmapImage(new Uri(loggedAccount.IconUri));
            UserLogin.Text = loggedAccount.Login;
            UserPassword.Text = loggedAccount.Password;

            playablePlaylist = 0;
            showingPlaylist = 0;
            mediaSelectedIndex = -1;
            LoopPlaylist = false;
            LoopTrack = false;
            #endregion

            foreach (Playlist p in loggedAccount.Playlists)
                AddPlaylist(p.Name);

            foreach (Media m in loggedAccount.Playlists[0].Medias)
                AddMediaToPlaylist(m);
        }

        #region Context menu functions
        private void CreateNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            DialogCreatePlaylist dialogWindow = new DialogCreatePlaylist();
            if ((bool)dialogWindow.ShowDialog())
            {
                string name = dialogWindow.PlaylistName;
                AddPlaylist(name);
                loggedAccount.Playlists.Add(new Playlist(name));
                Helper.SerializeAccountsAsync(loggedAccount);
            }
        }
        private void AddMediaToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (loggedAccount.Playlists.Count != 0)
            {
                OpenFileDialog fileDialog = new OpenFileDialog
                {
                    Filter = "MP3 files (*.mp3)|*.mp3|MP4 files (*.mp3)|*.mp4"
                };
                if ((bool)fileDialog.ShowDialog())
                {
                    Media music = new Media(fileDialog.FileName);
                    loggedAccount.Playlists[showingPlaylist].Medias.Add(music);
                    AddMediaToPlaylist(music);
                    Helper.SerializeAccountsAsync(loggedAccount);
                }
            }
        }
        private void RefreshPlaylist_Click(object sender, RoutedEventArgs e)
        {
            loggedAccount.Playlists[0].Medias.Clear();
            string[] files = Directory.GetFiles(Helper.path);
            if (files.Length != 0)
                foreach (string s in files)
                    if (Path.GetExtension(s) == ".mp3" || Path.GetExtension(s) == ".mp4")
                        loggedAccount.Playlists[0].Medias.Add(new Media(s));
            Helper.SerializeAccountsAsync(loggedAccount);
            FillCurrentPlaylist(0);
            MediaPlayer.Stop();
            MediaPlayer.Source = null;
            showingPlaylist = 0;
            playablePlaylist = 0;
            mediaSelectedIndex = -1;
        }
        #endregion


        #region Mediaplayer's buttons
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaSelectedIndex != -1)
            {
                if ((bool)PlayButton.IsChecked)
                {
                    PlayButton.Content = "&#xE768;";
                    MediaPlayer.Play();
                }
                else
                {
                    PlayButton.Content = "&#xE769;";
                    MediaPlayer.Pause();
                }
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            //проверяем, чтобы музыка была передана в медиаплеер
            if (MediaPlayer.Source != null)
                NextMusic();
        }
        void NextMusic()
        {

            mediaSelectedIndex++;
            if (LoopPlaylist)
            {
                if (mediaSelectedIndex == loggedAccount.Playlists[playablePlaylist].Medias.Count)
                    mediaSelectedIndex = 0;
                MediaPlayer.Source = new Uri(loggedAccount.Playlists[playablePlaylist].Medias[mediaSelectedIndex].Path);
                MediaPlayer.Play();
                PlayButton.IsChecked = true;
            }
            else
            {
                if (mediaSelectedIndex == loggedAccount.Playlists[playablePlaylist].Medias.Count)
                {
                    mediaSelectedIndex--;
                    MediaPlayer.Stop();
                    PlayButton.IsChecked = false;
                    MediaPlayer.Source = new Uri(loggedAccount.Playlists[playablePlaylist].Medias[mediaSelectedIndex].Path);
                }
                else
                {
                    MediaPlayer.Source = new Uri(loggedAccount.Playlists[playablePlaylist].Medias[mediaSelectedIndex].Path);
                    MediaPlayer.Play();
                    PlayButton.IsChecked = true;
                }
            }

        }
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            //проверяем, чтобы музыка была передана в медиаплеер
            if (MediaPlayer.Source != null)
                PreviousMusic();
        }
        void PreviousMusic()
        {

            mediaSelectedIndex--;
            if (LoopPlaylist)
            {
                if (mediaSelectedIndex == -1)
                    mediaSelectedIndex = loggedAccount.Playlists[playablePlaylist].Medias.Count - 1;
                MediaPlayer.Source = new Uri(loggedAccount.Playlists[playablePlaylist].Medias[mediaSelectedIndex].Path);
                MediaPlayer.Play();
                PlayButton.IsChecked = true;
            }
            else
            {
                if (mediaSelectedIndex == -1)
                {
                    mediaSelectedIndex++;
                    MediaPlayer.Stop();
                    PlayButton.IsChecked = false;
                    MediaPlayer.Source = new Uri(loggedAccount.Playlists[playablePlaylist].Medias[0].Path);
                }
                else
                {
                    MediaPlayer.Source = new Uri(loggedAccount.Playlists[playablePlaylist].Medias[mediaSelectedIndex].Path);
                    MediaPlayer.Play();
                    PlayButton.IsChecked = true;
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
        private void PlusTenSecButton_Click(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.Source != null)
            {
                TimeSpan time = TimeSpan.FromSeconds(10);
                if (TotalTime - MediaPlayer.Position >= time)
                    MediaPlayer.Position += time;
                else NextMusic();
            }
        }
        private void MinusTenSecButton_Click(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.Source != null)
            {
                TimeSpan time = TimeSpan.FromSeconds(10);
                if (MediaPlayer.Position >= time)
                    MediaPlayer.Position -= time;
                else PreviousMusic();
            }
        }

        #endregion

        #region Playlist's functions
        void AddMediaToPlaylist(Media media)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Height = 70,
                VerticalAlignment = VerticalAlignment.Center
            };
            var musicName = new TextBlock
            {
                Height = 30,
                FontSize = 22,
                VerticalAlignment = VerticalAlignment.Center
            };
            musicName.Text = Path.GetFileNameWithoutExtension(media.Path);
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
                Uid = media.Path
            };
            deleteButton.Click += DeleteMediaFromPlaylist_Click;
            panel.Children.Add(deleteButton);

            CurrentPlaylist.Items.Add(panel);
        }
        void FillCurrentPlaylist(int i)
        {
            CurrentPlaylist.Items.Clear();
            foreach (Media media in loggedAccount.Playlists[i].Medias)
                AddMediaToPlaylist(media);
            showingPlaylist = i;
        }
        private void CurrentPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index;
            if ((sender as ListBox).SelectedItem != null)
            {
                index = (sender as ListBox).SelectedIndex;
                if (index == mediaSelectedIndex && showingPlaylist == playablePlaylist)
                {
                    if ((bool)PlayButton.IsChecked)
                        MediaPlayer.Pause();
                    else MediaPlayer.Play();
                    PlayButton.IsChecked = !(bool)PlayButton.IsChecked;
                }
                else
                {
                    playablePlaylist = showingPlaylist;
                    MediaPlayer.Source = new Uri(loggedAccount.Playlists[playablePlaylist].Medias[index].Path);
                    MediaPlayer.Play();
                    PlayButton.IsChecked = true;
                    mediaSelectedIndex = index;
                }

                //снимает выделение после нажатия (вообще неплохо было бы просто убрать это самое выделение,
                // чтобы его видно не было и дополнительные переменные вводить)
                (sender as ListBox).SelectedItem = null;
            }
        }
        private void DeleteMediaFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            int index = loggedAccount.Playlists[showingPlaylist].Medias.FindIndex(m => m.Path == (sender as Button).Uid);
            loggedAccount.Playlists[showingPlaylist].Medias.RemoveAt(index);
            CurrentPlaylist.Items.RemoveAt(index);
            if (index == mediaSelectedIndex && showingPlaylist == playablePlaylist)
            {
                MediaPlayer.Stop();
                PlayButton.IsChecked = false;
                if (loggedAccount.Playlists[showingPlaylist].Medias.Count == 0)
                {
                    mediaSelectedIndex = -1;
                    MediaPlayer.Source = null;
                }
                else
                {
                    MediaPlayer.Source = new Uri(loggedAccount.Playlists[showingPlaylist].Medias[mediaSelectedIndex].Path);
                }
            }
            Helper.SerializeAccountsAsync(loggedAccount);
            MessageBox.Show("Песня была удалена из плейлиста");
        }
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
        private void PlaylistButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            var grid = (sender as Button).Content as Grid;
            var name = (grid.Children[0] as TextBlock).Text;
            int index = loggedAccount.Playlists.FindIndex(p => p.Name == name);
            if (index != showingPlaylist)
                FillCurrentPlaylist(index);
        }
        private void PlaylistButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PlaylistPanel.Children[0] != sender)
            {
                var panel = (sender as Button).Content as Grid;

                int deleteIndex = loggedAccount.Playlists.FindIndex(p => p.Name == (panel.Children[0] as TextBlock).Text);
                PlaylistPanel.Children.RemoveAt(deleteIndex);
                loggedAccount.Playlists.RemoveAt(deleteIndex);

                if (deleteIndex == playablePlaylist)
                {
                    showingPlaylist = 0;
                    playablePlaylist = 0;
                    mediaSelectedIndex = 0;
                    MediaPlayer.Stop();
                    MediaPlayer.Source = null;
                    PlayButton.IsChecked = false;
                }

                if (deleteIndex <= showingPlaylist)
                    showingPlaylist--;
                if (deleteIndex < playablePlaylist)
                    playablePlaylist--;

                FillCurrentPlaylist(playablePlaylist);

                Helper.SerializeAccountsAsync(loggedAccount);
            }
            else
            {
                MessageBox.Show("Нельзя удалить главный плейлист со всей музыкой");
            }
        }
        #endregion

        #region Mediaplayer's functions
        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (!LoopTrack)
            {
                mediaSelectedIndex++;
                if (LoopPlaylist)
                {
                    if (mediaSelectedIndex == loggedAccount.Playlists[playablePlaylist].Medias.Count)
                        mediaSelectedIndex = 0;
                    MediaPlayer.Source = new Uri(loggedAccount.Playlists[playablePlaylist].Medias[mediaSelectedIndex].Path);
                    MediaPlayer.Play();
                    PlayButton.IsChecked = true;
                }
                else
                {
                    if (mediaSelectedIndex == loggedAccount.Playlists[playablePlaylist].Medias.Count)
                    {
                        mediaSelectedIndex--;
                        MediaPlayer.Stop();
                        PlayButton.IsChecked = false;
                        MediaPlayer.Source = new Uri(loggedAccount.Playlists[playablePlaylist].Medias[mediaSelectedIndex].Path);
                        MessageBox.Show("Плейлист подошел к концу!");
                    }
                    else
                    {
                        MediaPlayer.Source = new Uri(loggedAccount.Playlists[playablePlaylist].Medias[mediaSelectedIndex].Path);
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

        #region MediaPlayer's Sliders
        bool isDragging = false;
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
            if (MediaPlayer.NaturalDuration.HasTimeSpan)
            {
                if (TotalTime.TotalSeconds > 0)
                {
                    if (!isDragging)
                    TimeSlider.Value = MediaPlayer.Position.TotalSeconds / TotalTime.TotalSeconds;
                }
            }
        }
        private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).SelectionEnd = e.NewValue;
            MediaDuration.Text = $"{MediaPlayer.Position.StripMilliseconds()} / {TotalTime.StripMilliseconds()}";
        }
        private void TimeSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDragging = true;
        }
        private void TimeSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (MediaPlayer.Source != null)
            {
                if (TotalTime.TotalSeconds > 0)
                {
                    MediaPlayer.Position = TimeSpan.FromSeconds((double)TimeSlider.Value * TotalTime.TotalSeconds);
                }
            }
            isDragging = false;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double volume = 100 * Math.Round((sender as Slider).Value, 2);
            if (Volume != null)
                Volume.Content = volume.ToString();
        }
        #endregion





        #region Account's buttons
        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            Helper.SerializeAccountsAsync(loggedAccount);
            RegisterLoginWindow window = new RegisterLoginWindow();
            window.Show();
            window.Activate();
            Close();
        }
        private void EditAccount_Click(object sender, RoutedEventArgs e)
        {
            EditAccount editAccount = new EditAccount(loggedAccount);
            if ((bool)editAccount.ShowDialog())
            {
                loggedAccount = editAccount.Editable;
                UserIcon.ImageSource = new BitmapImage(new Uri(loggedAccount.IconUri));
                UserLogin.Text = loggedAccount.Login;
                UserPassword.Text = loggedAccount.Password;
            }
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Helper.SerializeAccountsAsync(loggedAccount);
            DialogWindow window = new DialogWindow();
            window.Show();
            window.Activate();
            Close();
        }
        #endregion

    }
}