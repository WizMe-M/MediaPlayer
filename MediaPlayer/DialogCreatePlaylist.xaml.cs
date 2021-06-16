using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MediaPlayer
{
    /// <summary>
    /// Логика взаимодействия для DialogCreatePlaylist.xaml
    /// </summary>
    public partial class DialogCreatePlaylist : Window
    {
        public string PlaylistName { get; set; }
        List<string> directories;
        public DialogCreatePlaylist()
        {
            InitializeComponent();
            directories = new List<string>();
            string[] d = Directory.GetDirectories(Helper.musicPath);
            foreach (string s in d)
                directories.Add(System.IO.Path.GetDirectoryName(s));
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string playlistName = playlistNameTB.Text.Trim();
            string pattern = @"^[a-zA-Zа-яА-Я0-9 ]+$";
            if (!Regex.IsMatch(playlistName, pattern) || directories.Contains(playlistName))
            {
                MessageBox.Show("В поле \"Название плейлиста\" введены некорректные данные " +
                    "или плейлист с таким именем уже существует!", "ОШИБКА!", MessageBoxButton.OK, MessageBoxImage.Error);
                playlistNameTB.SelectionStart = 0;
                playlistNameTB.SelectionLength = playlistNameTB.Text.Length;
                playlistNameTB.Focus();
            }
            else
            {
                PlaylistName = playlistName;
                DialogResult = true;
            }
        }
    }
}
