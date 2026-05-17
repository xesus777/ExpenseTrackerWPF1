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
    public partial class AddEditBookPage : Page
    {
        private int? _bookId;
        public AddEditBookPage(int? bookId)
        {
            InitializeComponent();
            _bookId = bookId;
            GenresList.ItemsSource = Core.Context.Genres.ToList();
            if (bookId.HasValue)
            {
                var book = Core.Context.Books.First(b => b.BookID == bookId);
                TitleBox.Text = book.Title;
                DescriptionBox.Text = book.Description;
                CoverPathBox.Text = book.CoverImagePath;
                ContentBox.Text = book.Content;
                foreach (var item in GenresList.Items)
                {
                    var genre = item as Genres;
                    if (book.BookGenres.Any(bg => bg.GenreID == genre.GenreID))
                        GenresList.SelectedItems.Add(item);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Books book;
            if (_bookId.HasValue)
                book = Core.Context.Books.First(b => b.BookID == _bookId);
            else
            {
                book = new Books();
                Core.Context.Books.Add(book);
                book.AuthorID = MainWindow.CurrentUserID;
                book.CreatedAt = System.DateTime.Now;
                book.IsFrozen = false;
            }

            book.Title = TitleBox.Text;
            book.Description = DescriptionBox.Text;
            book.CoverImagePath = CoverPathBox.Text;
            book.Content = ContentBox.Text;

            Core.Context.SaveChanges();

            var existingGenres = Core.Context.BookGenres.Where(bg => bg.BookID == book.BookID).ToList();
            foreach (var eg in existingGenres)
                Core.Context.BookGenres.Remove(eg);

            foreach (Genres genre in GenresList.SelectedItems)
            {
                Core.Context.BookGenres.Add(new BookGenres { BookID = book.BookID, GenreID = genre.GenreID });
            }

            Core.Context.SaveChanges();
            MessageBox.Show("Сохранено");
            NavigationService.GoBack();
        }
    }
}
