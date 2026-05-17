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
using System.Windows.Shapes;

namespace WpfApp1
{
    public partial class SelectListWindow : Window
    {
        public string SelectedSection { get; private set; }
        public bool IsConfirmed { get; private set; }

        public SelectListWindow(string bookTitle)
        {
            InitializeComponent();
            BookTitleText.Text = $"Книга: {bookTitle}";
            Owner = Application.Current.MainWindow;
        }

        private void SectionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SectionsList.SelectedItem != null)
            {
                SelectedSection = (SectionsList.SelectedItem as ListBoxItem)?.Content.ToString();
                OkBtn.IsEnabled = true;
            }
            else
            {
                OkBtn.IsEnabled = false;
            }
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            Close();
        }
    }
}
