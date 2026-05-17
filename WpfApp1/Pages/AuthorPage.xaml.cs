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
    public partial class AuthorPage : Page
    {
        public AuthorPage()
        {
            InitializeComponent();
            LoadBooks();
        }

        private void LoadBooks()
        {
            var books = Core.Context.Books.Where(b => b.AuthorID == MainWindow.CurrentUserID).ToList();
            PublishedBooks.ItemsSource = books.Where(b => b.IsFrozen == false).ToList();
            FrozenBooks.ItemsSource = books.Where(b => b.IsFrozen == true).ToList();
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditBookPage(null));
        }

        private void EditBook_Click(object sender, RoutedEventArgs e)
        {
            var book = (sender as Button).Tag as Books;
            NavigationService.Navigate(new AddEditBookPage(book.BookID));
        }

        private void AppealFrozenBook_Click(object sender, RoutedEventArgs e)
        {
            var book = (sender as Button).Tag as Books;
            var request = new UnfreezeRequests
            {
                UserID = MainWindow.CurrentUserID,
                BookID = book.BookID,
                Reason = "Оспаривание заморозки книги",
                RequestDate = DateTime.Now,
                IsProcessed = false,
                IsApproved = null
            };
            Core.Context.UnfreezeRequests.Add(request);
            Core.Context.SaveChanges();
            MessageBox.Show("Заявка на разморозку книги отправлена");
        }
    }
}
