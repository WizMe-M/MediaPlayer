using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MediaPlayer
{
    /// <summary>
    /// Логика взаимодействия для CommonVersion.xaml
    /// </summary>
    public partial class CommonVersion : Window
    {
        public CommonVersion()
        {
            InitializeComponent();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MusicPlayer.LoadedBehavior = (bool)PlayButton.IsChecked ? MediaState.Play : MediaState.Pause;
        }
    }
}
