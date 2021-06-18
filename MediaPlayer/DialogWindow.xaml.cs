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
    /// Логика взаимодействия для DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();
        }

        private void OpenCommonVersion_Click(object sender, RoutedEventArgs e)
        {
            Close();
            CommonVersion window = new CommonVersion();
            window.Show();
            window.Activate();
        }

        private void OpenProVersion_Click(object sender, RoutedEventArgs e)
        {
            Close();
            RegisterLoginWindow window = new RegisterLoginWindow();
            window.Show();
        }
    }
}
