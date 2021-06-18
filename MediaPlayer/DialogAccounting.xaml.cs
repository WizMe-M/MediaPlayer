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
    /// Логика взаимодействия для DialogAccounting.xaml
    /// </summary>
    public partial class DialogAccounting : Window
    {
        bool Mode { get; set; }
        public Account Acc { get; set; }
        public DialogAccounting(bool Mode)
        {
            this.Mode = Mode;
            InitializeComponent();
            if (this.Mode)
            {
                Title = "Регистрация";
                ConfirmButton.Content = "Зарегистрировать нового пользователя";
            }
            else
            {
                Title = "Авторизация";
                ConfirmButton.Content = "Войти в аккаунт";
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text.Trim();
            string pass = PasswordBox.Password.Trim();
            if (Mode)
            {
                if (!Account.IsExist(login))
                {
                    Acc = new Account(login, pass);
                    Helper.SerializeAccountsAsync(Acc);
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Аккаунт с таким логином уже существует!" +
                    "\nВведите несуществующий логин", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoginTB.SelectionStart = 0;
                    LoginTB.SelectionLength = LoginTB.Text.Length;
                    LoginTB.Focus();
                }
            }
            else
            {
                Acc = Account.IsExist(login, pass);
                if (Acc != null)
                {
                    DialogResult = true;
                }
                else MessageBox.Show("Аккаунтов с такими данными не существует!" +
                    "\nВведите корректный логин и пароль", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }
        }
    }
}
