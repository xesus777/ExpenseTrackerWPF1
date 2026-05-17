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
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();

            if (MainWindow.CurrentUserID == 0)
            {
                MessageBox.Show("Для доступа к профилю необходимо авторизоваться");
                NavigationService?.Navigate(new AuthPage());
                return;
            }

            LoadProfile();
        }

        private void LoadProfile()
        {
            var user = Core.Context.Users.FirstOrDefault(u => u.UserID == MainWindow.CurrentUserID);
            if (user == null)
            {
                MessageBox.Show("Пользователь не найден");
                NavigationService?.Navigate(new AuthPage());
                return;
            }

            var role = Core.Context.Roles.FirstOrDefault(r => r.RoleID == user.RoleID);
            string roleName = role?.RoleName ?? "Не назначена";

            UserInfo.Text = $"{user.DisplayName} ({user.Login})\nEmail: {user.Email}\nРоль: {roleName}";
            UserReviews.ItemsSource = Core.Context.Reviews.Where(r => r.UserID == MainWindow.CurrentUserID).ToList();

            if (user.IsFrozen == true)
            {
                FrozenWarning.Text = $"Аккаунт заморожен. Причина: обратитесь к администратору";
                FrozenWarning.Visibility = Visibility.Visible;
                AppealFrozenBtn.Visibility = Visibility.Visible;
            }
            else
            {
                FrozenWarning.Visibility = Visibility.Collapsed;
                AppealFrozenBtn.Visibility = Visibility.Collapsed;
            }

            bool alreadyRequested = Core.Context.AuthorRoleRequests.Any(r => r.UserID == MainWindow.CurrentUserID && r.IsProcessed == false);
            if (roleName == "Автор" || alreadyRequested)
                AuthorRequestBtn.IsEnabled = false;
            else
                AuthorRequestBtn.IsEnabled = true;
        }

        private void AuthorRequest_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUserID == 0)
            {
                MessageBox.Show("Необходимо авторизоваться");
                NavigationService?.Navigate(new AuthPage());
                return;
            }

            var request = new AuthorRoleRequests
            {
                UserID = MainWindow.CurrentUserID,
                RequestDate = DateTime.Now,
                IsProcessed = false,
                IsApproved = null
            };
            Core.Context.AuthorRoleRequests.Add(request);
            Core.Context.SaveChanges();
            MessageBox.Show("Заявка отправлена");
            AuthorRequestBtn.IsEnabled = false;
        }

        private void AppealFrozen_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUserID == 0)
            {
                MessageBox.Show("Необходимо авторизоваться");
                NavigationService?.Navigate(new AuthPage());
                return;
            }

            var request = new UnfreezeRequests
            {
                UserID = MainWindow.CurrentUserID,
                BookID = null,
                Reason = "Оспаривание заморозки аккаунта",
                RequestDate = DateTime.Now,
                IsProcessed = false,
                IsApproved = null
            };
            Core.Context.UnfreezeRequests.Add(request);
            Core.Context.SaveChanges();
            MessageBox.Show("Заявка на разморозку отправлена");
        }
    }
}
