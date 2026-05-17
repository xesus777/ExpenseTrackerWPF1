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
    public partial class ReadingListsPage : Page
    {
        public ReadingListsPage()
        {
            InitializeComponent();
            GenreFilter.ItemsSource = Core.Context.Genres.ToList();
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

            if (GenreFilter.SelectedItem != null)
            {
                var genre = (Genres)GenreFilter.SelectedItem;
                query = query.Where(rl => rl.Books.BookGenres.Any(bg => bg.GenreID == genre.GenreID)).ToList();
            }

            if (SortBox.SelectedIndex == 0)
                query = query.OrderBy(rl => rl.Books.Title).ToList();
            else
                query = query.OrderByDescending(rl => rl.Books.Reviews.Average(r => (double?)r.Rating) ?? 0).ToList();

            BooksGrid.ItemsSource = query;
        }

        private void SectionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadLists();
        private void ApplyFilter_Click(object sender, RoutedEventArgs e) => LoadLists();

        private void MoveToList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            var readingList = combo.Tag as ReadingLists;
            var newSection = (combo.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (newSection != null)
            {
                readingList.Section = newSection;
                Core.Context.SaveChanges();
                LoadLists();
            }
        }

        private void OpenBook_Click(object sender, RoutedEventArgs e)
        {
            var book = (sender as Button).Tag as Books;
            NavigationService.Navigate(new BookPage(book.BookID));
        }
    }
}
