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
    public partial class BookPage : Page
    {
        private int _bookId;

        public bool IsAdmin { get; set; }

        public BookPage(int bookId)
        {
            InitializeComponent();
            _bookId = bookId;
            DataContext = this;
            LoadBook();
        }

        private void LoadBook()
        {
            var book = Core.Context.Books.First(b => b.BookID == _bookId);

            TitleText.Text = book.Title;
            AuthorText.Text = $"Автор: {book.Users.DisplayName}";
            GenresText.Text = "Жанры: " + string.Join(", ", book.BookGenres.Select(bg => bg.Genres.GenreName));
            DescriptionText.Text = book.Description;
            ContentText.Text = book.Content;

            if (!string.IsNullOrEmpty(book.CoverImagePath))
            {
                try
                {
                    CoverImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(book.CoverImagePath));
                }
                catch
                {
                    CoverImage.Source = null;
                }
            }

            if (MainWindow.CurrentUserID != 0)
            {
                var user = Core.Context.Users.FirstOrDefault(u => u.UserID == MainWindow.CurrentUserID);
                if (user != null)
                {
                    var role = Core.Context.Roles.FirstOrDefault(r => r.RoleID == user.RoleID);
                    IsAdmin = role?.RoleName == "Администратор";
                }
            }

            if (IsAdmin)
            {
                FreezeBookBtn.Visibility = Visibility.Visible;
            }

            ReviewsList.ItemsSource = book.Reviews.ToList();
        }

        private void ComplainBook_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUserID == 0)
            {
                MessageBox.Show("Необходимо авторизоваться");
                return;
            }

            var book = Core.Context.Books.First(b => b.BookID == _bookId);
            var complaintWindow = new ComplaintWindow($"Жалоба на книгу: {book.Title}");
            complaintWindow.Owner = Application.Current.MainWindow;
            complaintWindow.ShowDialog();

            if (complaintWindow.IsSent && !string.IsNullOrEmpty(complaintWindow.Reason))
            {
                var complaint = new Complaints
                {
                    UserID = MainWindow.CurrentUserID,
                    BookID = _bookId,
                    ReviewID = null,
                    Reason = complaintWindow.Reason,
                    CreatedAt = DateTime.Now
                };
                Core.Context.Complaints.Add(complaint);
                Core.Context.SaveChanges();
                MessageBox.Show("Жалоба на книгу отправлена");
            }
        }

        private void ComplainAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUserID == 0)
            {
                MessageBox.Show("Необходимо авторизоваться");
                return;
            }

            var book = Core.Context.Books.First(b => b.BookID == _bookId);
            var complaintWindow = new ComplaintWindow($"Жалоба на автора: {book.Users.DisplayName}");
            complaintWindow.Owner = Application.Current.MainWindow;
            complaintWindow.ShowDialog();

            if (complaintWindow.IsSent && !string.IsNullOrEmpty(complaintWindow.Reason))
            {
                var complaint = new Complaints
                {
                    UserID = MainWindow.CurrentUserID,
                    BookID = _bookId,
                    ReviewID = null,
                    Reason = complaintWindow.Reason,
                    CreatedAt = DateTime.Now
                };
                Core.Context.Complaints.Add(complaint);
                Core.Context.SaveChanges();
                MessageBox.Show("Жалоба на автора отправлена");
            }
        }

        private void FreezeBook_Click(object sender, RoutedEventArgs e)
        {
            var book = Core.Context.Books.First(b => b.BookID == _bookId);
            book.IsFrozen = true;
            Core.Context.SaveChanges();
            MessageBox.Show("Книга заморожена");
            LoadBook();
        }

        private void ComplainReview_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUserID == 0)
            {
                MessageBox.Show("Необходимо авторизоваться");
                return;
            }

            var review = (sender as Button).Tag as Reviews;
            var book = Core.Context.Books.First(b => b.BookID == _bookId);

            var complaintWindow = new ComplaintWindow($"Жалоба на отзыв пользователя {review.Users.DisplayName} к книге {book.Title}");
            complaintWindow.Owner = Application.Current.MainWindow;
            complaintWindow.ShowDialog();

            if (complaintWindow.IsSent && !string.IsNullOrEmpty(complaintWindow.Reason))
            {
                var complaint = new Complaints
                {
                    UserID = MainWindow.CurrentUserID,
                    BookID = null,
                    ReviewID = review.ReviewID,
                    Reason = complaintWindow.Reason,
                    CreatedAt = DateTime.Now
                };
                Core.Context.Complaints.Add(complaint);
                Core.Context.SaveChanges();
                MessageBox.Show("Жалоба на отзыв отправлена");
            }
        }

        private void FreezeReview_Click(object sender, RoutedEventArgs e)
        {
            var review = (sender as Button).Tag as Reviews;
            Core.Context.Reviews.Remove(review);
            Core.Context.SaveChanges();
            MessageBox.Show("Отзыв заморожен (удалён)");
            LoadBook();
        }

        private void SubmitReview_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUserID == 0)
            {
                MessageBox.Show("Необходимо авторизоваться");
                return;
            }

            if (RatingBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите оценку");
                return;
            }

            if (string.IsNullOrWhiteSpace(ReviewText.Text))
            {
                MessageBox.Show("Введите текст отзыва");
                return;
            }

            var review = new Reviews
            {
                UserID = MainWindow.CurrentUserID,
                BookID = _bookId,
                ReviewText = ReviewText.Text,
                Rating = int.Parse((RatingBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "5"),
                CreatedAt = DateTime.Now
            };
            Core.Context.Reviews.Add(review);
            Core.Context.SaveChanges();
            MessageBox.Show("Отзыв добавлен");
            LoadBook();
            ReviewText.Text = "";
        }
    }
}
