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

    public class BookDisplay
    {
        public Books Book { get; set; }
        public double AverageRating { get; set; }
        public string RatingText => $"{AverageRating:F1} ★";
    }

    public partial class CatalogPage : Page
    {
        public CatalogPage()
        {
            InitializeComponent();

            var genresList = Core.Context.Genres.ToList();
            genresList.Insert(0, new Genres { GenreID = 0, GenreName = "Все жанры" });
            GenreFilter.ItemsSource = genresList;
            GenreFilter.SelectedIndex = 0;

            LoadBooks();
        }

        private void LoadBooks()
        {
            var query = Core.Context.Books.Where(b => b.IsFrozen == false).ToList();

            string search = SearchBox.Text.ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>
                    b.Title.ToLower().Contains(search.ToLower()) ||
                    (b.Users != null && b.Users.DisplayName.ToLower().Contains(search.ToLower()))
                ).ToList();
            }

            var selectedGenre = GenreFilter.SelectedItem as Genres;
            if (selectedGenre != null && selectedGenre.GenreID != 0)
            {
                var bookIds = Core.Context.BookGenres
                    .Where(bg => bg.GenreID == selectedGenre.GenreID)
                    .Select(bg => bg.BookID)
                    .ToList();
                query = query.Where(b => bookIds.Contains(b.BookID)).ToList();
            }

            if (SortBox.SelectedIndex == 0)
            {
                query = query.OrderBy(b => b.Title).ToList();
            }
            else
            {
                query = query.OrderByDescending(b => b.Reviews.Average(r => (double?)r.Rating) ?? 0).ToList();
            }

            var displayList = query.Select(b => new BookDisplay
            {
                Book = b,
                AverageRating = b.Reviews.Count == 0 ? 0 : b.Reviews.Average(r => r.Rating)
            }).ToList();

            BooksGrid.ItemsSource = displayList;
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadBooks();
        }

        private void OpenBook_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).Tag as BookDisplay;
            NavigationService?.Navigate(new BookPage(item.Book.BookID));
        }

        private void AddToList_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).Tag as BookDisplay;
            var book = item.Book;

            if (MainWindow.CurrentUserID == 0)
            {
                MessageBox.Show("Необходимо авторизоваться, чтобы добавлять книги в списки");
                return;
            }

            var selectWindow = new SelectListWindow(book.Title);
            selectWindow.ShowDialog();

            if (selectWindow.IsConfirmed && !string.IsNullOrEmpty(selectWindow.SelectedSection))
            {
                var existing = Core.Context.ReadingLists
                    .FirstOrDefault(rl => rl.UserID == MainWindow.CurrentUserID && rl.BookID == book.BookID);

                if (existing != null)
                    Core.Context.ReadingLists.Remove(existing);

                Core.Context.ReadingLists.Add(new ReadingLists
                {
                    UserID = MainWindow.CurrentUserID,
                    BookID = book.BookID,
                    Section = selectWindow.SelectedSection,
                    AddedAt = DateTime.Now
                });

                Core.Context.SaveChanges();
                MessageBox.Show($"Книга добавлена в список \"{selectWindow.SelectedSection}\"");
            }
        }
    }
}
