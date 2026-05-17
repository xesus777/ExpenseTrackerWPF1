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
    public class ReadingListDisplay
    {
        public ReadingLists ReadingList { get; set; }
        public double AverageRating { get; set; }
        public string RatingText => $"{AverageRating:F1} ★";
    }

    public partial class ReadingListsPage : Page
    {
        public ReadingListsPage()
        {
            InitializeComponent();

            var genresList = Core.Context.Genres.ToList();
            genresList.Insert(0, new Genres { GenreID = 0, GenreName = "Все жанры" });
            GenreFilter.ItemsSource = genresList;
            GenreFilter.SelectedIndex = 0;

            LoadLists();
        }

        private string CurrentSection => (SectionSelector.SelectedItem as ListBoxItem)?.Content.ToString() ?? "В планах";

        private void LoadLists()
        {
            var query = Core.Context.ReadingLists
                .Where(rl => rl.UserID == MainWindow.CurrentUserID && rl.Section == CurrentSection)
                .ToList();

            string search = SearchBox.Text;
            if (!string.IsNullOrEmpty(search))
                query = query.Where(rl => rl.Books.Title.Contains(search) || rl.Books.Users.DisplayName.Contains(search)).ToList();

            var selectedGenre = GenreFilter.SelectedItem as Genres;
            if (selectedGenre != null && selectedGenre.GenreID != 0)
            {
                query = query.Where(rl => rl.Books.BookGenres.Any(bg => bg.GenreID == selectedGenre.GenreID)).ToList();
            }

            if (SortBox.SelectedIndex == 0)
                query = query.OrderBy(rl => rl.Books.Title).ToList();
            else
                query = query.OrderByDescending(rl => rl.Books.Reviews.Average(r => (double?)r.Rating) ?? 0).ToList();

            var displayList = query.Select(rl => new ReadingListDisplay
            {
                ReadingList = rl,
                AverageRating = rl.Books.Reviews.Count == 0 ? 0 : rl.Books.Reviews.Average(r => r.Rating)
            }).ToList();

            BooksGrid.ItemsSource = displayList;
        }

        private void SectionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadLists();
        private void ApplyFilter_Click(object sender, RoutedEventArgs e) => LoadLists();

        private void MoveToList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            var display = combo?.Tag as ReadingListDisplay;
            if (display == null) return;

            var newSection = (combo?.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (newSection != null && display.ReadingList.Section != newSection)
            {
                display.ReadingList.Section = newSection;
                Core.Context.SaveChanges();
                LoadLists();
            }
        }

        private void OpenBook_Click(object sender, RoutedEventArgs e)
        {
            var display = (sender as Button)?.Tag as ReadingListDisplay;
            if (display?.ReadingList?.Books != null)
                NavigationService?.Navigate(new BookPage(display.ReadingList.Books.BookID));
        }
    }
}
