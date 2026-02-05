using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfApp1.Pages;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public static List<Products> AllProducts { get; set; }
        public static List<CartItem> Cart { get; set; } = new List<CartItem>();

        public MainWindow()
        {
            InitializeComponent();
            LoadProducts();
            MainFrame.Navigate(new ProductsPage());
        }

        private void LoadProducts()
        {
            AllProducts = Core.Context.Products.ToList();
        }

        public static void AddToCart(Products product)
        {
            var item = Cart.FirstOrDefault(c => c.ProductId == product.Id);
            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                Cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    Quantity = 1,
                    Product = product
                });
            }
        }

        public static void RemoveFromCart(CartItem item)
        {
            Cart.Remove(item);
        }

        public static decimal GetTotalPrice()
        {
            return Cart.Sum(item => item.Product.Price * item.Quantity);
        }
    }
}
