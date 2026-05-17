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

namespace WpfApp1.Other
{
    public partial class ChangePasswordWindow : Window
    {
        public string NewPassword { get; private set; }
        public bool IsConfirmed { get; private set; }

        public ChangePasswordWindow(string userLogin)
        {
            InitializeComponent();
            UserLoginText.Text = $"Пользователь: {userLogin}";
            IsConfirmed = false;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewPasswordBox.Password))
            {
                MessageBox.Show("Введите новый пароль");
                return;
            }

            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }

            NewPassword = NewPasswordBox.Password;
            IsConfirmed = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            Close();
        }
    }
}
