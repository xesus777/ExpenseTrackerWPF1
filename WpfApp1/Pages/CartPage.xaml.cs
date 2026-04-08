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
            LoadCart();
        }

        private void LoadCart()
        {
            if (MainWindow.CurrentUserId == 0)
            {
                NavigationService.Navigate(new LoginPage());
                return;
            }

            var cartItems = (from c in Core.Context.Carts
                             join p in Core.Context.Products on c.ProductId equals p.Id
                             join m in Core.Context.Manufacturers on p.ManufacturerId equals m.Id
                             where c.UserId == MainWindow.CurrentUserId
                             select new CartItemInfo
                             {
                                 CartId = c.Id,
                                 ProductId = p.Id,
                                 ProductName = p.Name,
                                 ManufacturerName = m.Name,
                                 Price = (decimal)(p.Price - (p.Price * p.Discount / 100)),
                                 Quantity = (int)c.Quantity
                             }).ToList();

            LvCart.ItemsSource = cartItems;

            var total = cartItems.Sum(i => i.Price * i.Quantity);
            TxtTotal.Text = $"Итого: {total:F2} руб";
        }

        private void Increment_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.Tag as CartItemInfo;
            if (item != null)
            {
                var cart = Core.Context.Carts.FirstOrDefault(c => c.Id == item.CartId);
                if (cart != null)
                {
                    cart.Quantity++;
                    Core.Context.SaveChanges();
                    LoadCart();
                }
            }
        }

        private void Decrement_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.Tag as CartItemInfo;
            if (item != null && item.Quantity > 1)
            {
                var cart = Core.Context.Carts.FirstOrDefault(c => c.Id == item.CartId);
                if (cart != null)
                {
                    cart.Quantity--;
                    Core.Context.SaveChanges();
                    LoadCart();
                }
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.Tag as CartItemInfo;
            if (item != null)
            {
                var cart = Core.Context.Carts.FirstOrDefault(c => c.Id == item.CartId);
                if (cart != null)
                {
                    Core.Context.Carts.Remove(cart);
                    Core.Context.SaveChanges();
                    LoadCart();
                }
            }
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            var cartItems = LvCart.ItemsSource as List<CartItemInfo>;
            if (cartItems == null || !cartItems.Any())
            {
                MessageBox.Show("Корзина пуста");
                return;
            }

            var checkoutWindow = new Window
            {
                Title = "Оформление заказа",
                Width = 350,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };

            stackPanel.Children.Add(new TextBlock { Text = "Дата доставки (не более 7 дней):", Margin = new Thickness(0, 0, 0, 5) });
            var datePicker = new DatePicker { Margin = new Thickness(0, 0, 0, 10) };
            datePicker.SelectedDate = DateTime.Now.AddDays(1);
            datePicker.DisplayDateStart = DateTime.Now.AddDays(1);
            datePicker.DisplayDateEnd = DateTime.Now.AddDays(7);
            stackPanel.Children.Add(datePicker);

            stackPanel.Children.Add(new TextBlock { Text = "Способ оплаты:", Margin = new Thickness(0, 0, 0, 5) });
            var paymentCombo = new ComboBox { Margin = new Thickness(0, 0, 0, 10) };
            paymentCombo.Items.Add("Наличные");
            paymentCombo.Items.Add("Карта");
            paymentCombo.Items.Add("Онлайн");
            paymentCombo.SelectedIndex = 0;
            stackPanel.Children.Add(paymentCombo);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
            var confirmBtn = new Button { Content = "Подтвердить", Width = 100, Height = 30, Margin = new Thickness(5) };
            var cancelBtn = new Button { Content = "Отмена", Width = 100, Height = 30, Margin = new Thickness(5) };
            buttonPanel.Children.Add(confirmBtn);
            buttonPanel.Children.Add(cancelBtn);
            stackPanel.Children.Add(buttonPanel);

            checkoutWindow.Content = stackPanel;

            confirmBtn.Click += (s, args) =>
            {
                if (datePicker.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату доставки");
                    return;
                }

                var order = new Orders
                {
                    UserId = MainWindow.CurrentUserId,
                    OrderDate = DateTime.Now,
                    DeliveryDate = datePicker.SelectedDate.Value,
                    PaymentMethod = paymentCombo.SelectedItem.ToString(),
                    Status = "Pending",
                    TotalAmount = cartItems.Sum(i => i.Price * i.Quantity)
                };

                Core.Context.Orders.Add(order);
                Core.Context.SaveChanges();

                foreach (var item in cartItems)
                {
                    Core.Context.OrderItems.Add(new OrderItems
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    });
                }

                var carts = Core.Context.Carts.Where(c => c.UserId == MainWindow.CurrentUserId).ToList();
                Core.Context.Carts.RemoveRange(carts);
                Core.Context.SaveChanges();

                MessageBox.Show($"Заказ №{order.Id} оформлен!");
                checkoutWindow.Close();
                NavigationService.Navigate(new ClientAccountPage());
            };

            cancelBtn.Click += (s, args) => checkoutWindow.Close();

            checkoutWindow.ShowDialog();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }

    public class CartItemInfo
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ManufacturerName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
}
