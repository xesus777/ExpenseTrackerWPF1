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
using WpfApp1.Pages;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public static int CurrentUserID { get; set; }
        public static bool IsAdmin { get; set; }
        public static bool IsAuthor { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new AuthPage());
        }

        public void UpdateSidebar()
        {
            var user = Core.Context.Users.FirstOrDefault(u => u.UserID == CurrentUserID);
            if (user == null) return;

            var role = Core.Context.Roles.FirstOrDefault(r => r.RoleID == user.RoleID);
            string roleName = role?.RoleName ?? "";

            if (user.IsFrozen == true)
                FrozenWarning.Visibility = Visibility.Visible;
            else
                FrozenWarning.Visibility = Visibility.Collapsed;

            if (roleName == "Администратор")
            {
                AdminBtn.Visibility = Visibility.Visible;
                IsAdmin = true;
            }
            else
            {
                AdminBtn.Visibility = Visibility.Collapsed;
                IsAdmin = false;
            }

            if (roleName == "Автор")
            {
                AuthorBtn.Visibility = Visibility.Visible;
                IsAuthor = true;
            }
            else
            {
                AuthorBtn.Visibility = Visibility.Collapsed;
                IsAuthor = false;
            }
        }

        private void CatalogBtn_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new CatalogPage());
        private void ReadingListsBtn_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ReadingListsPage());
        private void AdminBtn_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new AdminPage());
        private void AuthorBtn_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new AuthorPage());

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUserID == 0)
            {
                MainFrame.Navigate(new AuthPage());
            }
            else
            {
                MainFrame.Navigate(new ProfilePage());
            }
        }
    }
}
