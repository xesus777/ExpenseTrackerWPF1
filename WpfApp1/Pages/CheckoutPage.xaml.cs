using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class CheckoutPage : Page
    {
        public CheckoutPage()
        {
            InitializeComponent();
            LoadOrderItems();
        }

        private void LoadOrderItems()
        {
            OrderItemsPanel.Children.Clear();

            foreach (var item in MainWindow.Cart)
            {
                var stack = new StackPanel
                {
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var text = new TextBlock
                {
                    Text = $"{item.Product.Name} - {item.Quantity} шт × {item.Product.Price:C} = {item.Product.Price * item.Quantity:C}",
                    FontSize = 14
                };

                stack.Children.Add(text);
                OrderItemsPanel.Children.Add(stack);
            }

            FinalTotalText.Text = $"Итого: {MainWindow.GetTotalPrice():C}";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage());
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(AddressTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (MainWindow.Cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста!");
                return;
            }

            var order = new Orders
            {
                CustomerFullName = FullNameTextBox.Text,
                Email = EmailTextBox.Text,
                DeliveryAddress = AddressTextBox.Text,
                TotalAmount = MainWindow.GetTotalPrice()
            };

            Core.Context.Orders.Add(order);
            Core.Context.SaveChanges();

            foreach (var cartItem in MainWindow.Cart)
            {
                var orderItem = new OrderItems
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price
                };
                Core.Context.OrderItems.Add(orderItem);
            }

            Core.Context.SaveChanges();

            MainWindow.Cart.Clear();

            MessageBox.Show("Заказ оформлен успешно!");
            NavigationService.Navigate(new ProductsPage());
        }

    }
}
