using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using WpfApp1.Pages;
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1
{
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            ProductsPanel.Children.Clear();

            foreach (var product in MainWindow.AllProducts)
            {
                var border = new Border
                {
                    BorderBrush = System.Windows.Media.Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(10),
                    Width = 200,
                    Background = System.Windows.Media.Brushes.White
                };

                var stack = new StackPanel();

                
                var image = new System.Windows.Controls.Image
                {
                    Height = 120,
                    Source = new BitmapImage(new System.Uri(product.ImageUrl, System.UriKind.RelativeOrAbsolute)),
                    Stretch = System.Windows.Media.Stretch.Uniform
                };
                stack.Children.Add(image);

                
                var nameText = new TextBlock
                {
                    Text = product.Name,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5),
                    TextWrapping = TextWrapping.Wrap
                };
                stack.Children.Add(nameText);

                
                var priceText = new TextBlock
                {
                    Text = $"Цена: {product.Price:C}",
                    FontSize = 14,
                    Margin = new Thickness(5),
                    Foreground = System.Windows.Media.Brushes.Green
                };
                stack.Children.Add(priceText);

                
                var button = new Button
                {
                    Content = "Добавить в корзину",
                    Margin = new Thickness(5),
                    Padding = new Thickness(10, 5, 10, 5),
                    Background = System.Windows.Media.Brushes.Blue,
                    Foreground = System.Windows.Media.Brushes.White
                };

                button.Click += (s, e) =>
                {
                    MainWindow.AddToCart(product);
                    MessageBox.Show($"{product.Name} добавлен в корзину!");
                };

                stack.Children.Add(button);

                border.Child = stack;
                ProductsPanel.Children.Add(border);
            }
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage());
        }
    }
}
