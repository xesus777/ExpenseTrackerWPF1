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
using WpfApp1.Other;

namespace WpfApp1.Pages
{
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            ComplaintsList.ItemsSource = Core.Context.Complaints.ToList();
            UnfreezeRequestsList.ItemsSource = Core.Context.UnfreezeRequests.Where(u => u.IsProcessed == false).ToList();
            AuthorRequestsList.ItemsSource = Core.Context.AuthorRoleRequests.Where(a => a.IsProcessed == false).ToList();
            UsersGrid.ItemsSource = Core.Context.Users.ToList();
        }

        private void SearchUsers_Click(object sender, RoutedEventArgs e)
        {
            var search = SearchBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(search))
            {
                UsersGrid.ItemsSource = Core.Context.Users.ToList();
            }
            else
            {
                var filtered = Core.Context.Users.Where(u => u.Login.ToLower().Contains(search)).ToList();
                UsersGrid.ItemsSource = filtered;
            }
        }

        private void AcceptComplaint_Click(object sender, RoutedEventArgs e)
        {
            var complaint = (sender as Button).Tag as Complaints;
            if (complaint == null) return;

            if (complaint.BookID != null)
            {
                var book = Core.Context.Books.FirstOrDefault(b => b.BookID == complaint.BookID);
                if (book != null)
                {
                    book.IsFrozen = true;
                    MessageBox.Show($"Книга \"{book.Title}\" заморожена");
                }
            }

            if (complaint.ReviewID != null)
            {
                var review = Core.Context.Reviews.FirstOrDefault(r => r.ReviewID == complaint.ReviewID);
                if (review != null)
                {
                    Core.Context.Reviews.Remove(review);
                    MessageBox.Show("Отзыв удалён");
                }
            }

            Core.Context.Complaints.Remove(complaint);
            Core.Context.SaveChanges();
            LoadData();
        }

        private void RejectComplaint_Click(object sender, RoutedEventArgs e)
        {
            var complaint = (sender as Button).Tag as Complaints;
            if (complaint == null) return;

            Core.Context.Complaints.Remove(complaint);
            Core.Context.SaveChanges();
            LoadData();
            MessageBox.Show("Жалоба отклонена");
        }

        private void AcceptUnfreezeRequest_Click(object sender, RoutedEventArgs e)
        {
            var request = (sender as Button).Tag as UnfreezeRequests;
            if (request == null) return;

            request.IsProcessed = true;
            request.IsApproved = true;

            if (request.BookID != null)
            {
                var book = Core.Context.Books.FirstOrDefault(b => b.BookID == request.BookID);
                if (book != null)
                {
                    book.IsFrozen = false;
                    MessageBox.Show($"Книга \"{book.Title}\" разморожена");
                }
            }
            else
            {
                var user = Core.Context.Users.FirstOrDefault(u => u.UserID == request.UserID);
                if (user != null)
                {
                    user.IsFrozen = false;
                    MessageBox.Show($"Аккаунт пользователя {user.Login} разморожен");
                }
            }

            Core.Context.SaveChanges();
            LoadData();
        }

        private void RejectUnfreezeRequest_Click(object sender, RoutedEventArgs e)
        {
            var request = (sender as Button).Tag as UnfreezeRequests;
            if (request == null) return;

            request.IsProcessed = true;
            request.IsApproved = false;
            Core.Context.SaveChanges();
            LoadData();
            MessageBox.Show("Заявка на разморозку отклонена");
        }

        private void AcceptAuthorRequest_Click(object sender, RoutedEventArgs e)
        {
            var request = (sender as Button).Tag as AuthorRoleRequests;
            if (request == null) return;

            request.IsProcessed = true;
            request.IsApproved = true;

            var user = Core.Context.Users.FirstOrDefault(u => u.UserID == request.UserID);
            if (user != null)
            {
                var authorRole = Core.Context.Roles.FirstOrDefault(r => r.RoleName == "Автор");
                if (authorRole != null)
                {
                    user.RoleID = authorRole.RoleID;
                    MessageBox.Show($"Пользователю {user.Login} назначена роль Автор");
                }
            }

            Core.Context.SaveChanges();
            LoadData();
        }

        private void RejectAuthorRequest_Click(object sender, RoutedEventArgs e)
        {
            var request = (sender as Button).Tag as AuthorRoleRequests;
            if (request == null) return;

            request.IsProcessed = true;
            request.IsApproved = false;
            Core.Context.SaveChanges();
            LoadData();
            MessageBox.Show("Заявка на роль автора отклонена");
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).Tag as Users;
            if (user == null) return;

            var passwordWindow = new ChangePasswordWindow(user.Login);
            passwordWindow.Owner = Application.Current.MainWindow;
            passwordWindow.ShowDialog();

            if (passwordWindow.IsConfirmed && !string.IsNullOrEmpty(passwordWindow.NewPassword))
            {
                user.PasswordHash = passwordWindow.NewPassword;
                Core.Context.SaveChanges();
                MessageBox.Show($"Пароль для пользователя {user.Login} изменён");
            }
        }

        private void ChangeRole_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).Tag as Users;
            if (user == null) return;

            var roleWindow = new ChangeRoleWindow(user);
            roleWindow.Owner = Application.Current.MainWindow;
            roleWindow.ShowDialog();
            LoadData();
        }
    }

}
