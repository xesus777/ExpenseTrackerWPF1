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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TxtLogin.Text.Trim();
            string password = TxtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                TxtError.Text = "Введите логин и пароль";
                return;
            }

            var user = Core.Context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user == null)
            {
                TxtError.Text = "Неверный логин или пароль";
                return;
            }

            if (user.IsFrozen == true)
            {
                TxtError.Text = "Аккаунт заморожен";
                return;
            }

            var role = Core.Context.Roles.FirstOrDefault(r => r.Id == user.RoleId);

            MainWindow.CurrentUserId = user.Id;
            MainWindow.CurrentUserRole = role?.Name;
            MainWindow.CurrentUserName = user.FullName;

            MainWindow.Instance.UpdateUIBasedOnAuth();

            var startPage = new StartPage();
            MainWindow.Instance.NavigateTo(startPage);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
