using Microsoft.Win32;
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
    /// Логика взаимодействия для EditAccount.xaml
    /// </summary>
    public partial class EditAccount : Window
    {
        public Account Editable { get; set; }
        int index;
        public EditAccount(Account account)
        {
            InitializeComponent();
            index = Helper.DeserializeAccount().FindIndex(a => a.Login == account.Login);
            Editable = account;
            Icon.Source = new BitmapImage(new Uri(Editable.IconUri));
            LoginTB.Text = Editable.Login;
            PasswordTB.Text = Editable.Password;
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Images (*.png, *.jpg, *.jpeg)|*.png;*.jpg;*.jpeg"
            };
            if ((bool)fileDialog.ShowDialog())
            {
                Icon.Source = new BitmapImage(new Uri(fileDialog.FileName));
                Editable.IconUri = fileDialog.FileName;
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Editable.Login = LoginTB.Text;
            Editable.Password = PasswordTB.Text;
            Helper.SerializeAccountsAsync(Editable, index);
            DialogResult = true;
        }
    }
}
