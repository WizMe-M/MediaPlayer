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
    /// Логика взаимодействия для RegisterLoginWindow.xaml
    /// </summary>
    public partial class RegisterLoginWindow : Window
    {
        public RegisterLoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Confirm(false);
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            Confirm(true);
        }

        void Confirm(bool mode)
        {
            DialogAccounting dialog = new DialogAccounting(mode);
            if ((bool)dialog.ShowDialog())
            {
                ProVersion window = new ProVersion(dialog.Acc);
                window.Show();
                window.Activate();
                Close();
            }
        }
    }
}
