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
    public partial class CartPage : Page
    {
        public CartPage()
        {
            InitializeComponent();
            LoadCartItems();
            UpdateTotal();
        }

        private void LoadCartItems()
        {
            CartItemsPanel.Children.Clear();

            foreach (var item in MainWindow.Cart)
            {
                var border = new Border
                {
                    BorderBrush = System.Windows.Media.Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(0, 0, 0, 10),
                    Background = System.Windows.Media.Brushes.White,
                    Padding = new Thickness(10)
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

                var image = new Image
                {
                    Width = 70,
                    Height = 70,
                    Source = new BitmapImage(new System.Uri(item.Product.ImageUrl, System.UriKind.RelativeOrAbsolute)),
                    Stretch = System.Windows.Media.Stretch.Uniform
                };
                Grid.SetColumn(image, 0);
                grid.Children.Add(image);

                var nameText = new TextBlock
                {
                    Text = item.Product.Name,
                    FontSize = 16,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 0, 0, 0)
                };
                Grid.SetColumn(nameText, 1);
                grid.Children.Add(nameText);

                var quantityText = new TextBlock
                {
                    Text = $"Количество: {item.Quantity}",
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(quantityText, 2);
                grid.Children.Add(quantityText);

                var priceText = new TextBlock
                {
                    Text = $"Сумма: {item.Product.Price * item.Quantity:C}",
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(priceText, 3);
                grid.Children.Add(priceText);

                
                var removeButton = new Button
                {
                    Content = "Удалить",
                    Padding = new Thickness(5),
                    Background = System.Windows.Media.Brushes.Red,
                    Foreground = System.Windows.Media.Brushes.White
                };
                removeButton.Click += (s, e) =>
                {
                    MainWindow.RemoveFromCart(item);
                    LoadCartItems();
                    UpdateTotal();
                };
                Grid.SetColumn(removeButton, 4);
                grid.Children.Add(removeButton);

                border.Child = grid;
                CartItemsPanel.Children.Add(border);
            }
        }

        private void UpdateTotal()
        {
            var total = MainWindow.GetTotalPrice();
            var count = MainWindow.Cart.Count;

            TotalText.Text = $"Общая сумма: {total:C}";
            CountText.Text = $"Товаров: {count} шт";

            CheckoutButton.IsEnabled = count > 0;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductsPage());
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CheckoutPage());
        }
    }
}
