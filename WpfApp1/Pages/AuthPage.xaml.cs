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
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            var user = Core.Context.Users.FirstOrDefault(u => u.Login == LoginBox.Text && u.PasswordHash == PasswordBox.Password);
            if (user != null)
            {
                MainWindow.CurrentUserID = user.UserID;
                (Application.Current.MainWindow as MainWindow).UpdateSidebar();
                (Application.Current.MainWindow as MainWindow).MainFrame.Navigate(new CatalogPage());
            }
            else
                ErrorText.Text = "Неверный логин или пароль";
        }

        private void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Core.Context.Users.Any(u => u.Login == RegLogin.Text))
            {
                ErrorText.Text = "Логин занят";
                return;
            }
            var newUser = new Users
            {
                Login = RegLogin.Text,
                PasswordHash = RegPassword.Password,
                Email = RegEmail.Text,
                DisplayName = RegDisplayName.Text,
                RoleID = Core.Context.Roles.First(r => r.RoleName == "Читатель").RoleID,
                IsFrozen = false,
                CreatedAt = System.DateTime.Now
            };
            Core.Context.Users.Add(newUser);
            Core.Context.SaveChanges();
            ErrorText.Text = "Регистрация успешна, войдите";
        }
    }
}
