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
    public partial class ProductsPage : Page
    {
        private List<ProductInfo> _allProducts;

        public ProductsPage()
        {
            InitializeComponent();
            LoadFilters();
            LoadProducts();
        }

        private void LoadFilters()
        {
            var types = Core.Context.ProductTypes.ToList();
            types.Insert(0, new ProductTypes { Id = 0, Name = "Все типы" });
            CmbProductType.ItemsSource = types;

            var mans = Core.Context.Manufacturers.ToList();
            mans.Insert(0, new Manufacturers { Id = 0, Name = "Все производители" });
            CmbManufacturer.ItemsSource = mans;
        }

        private void LoadProducts()
        {
            var query = from p in Core.Context.Products
                        join m in Core.Context.Manufacturers on p.ManufacturerId equals m.Id
                        join pt in Core.Context.ProductTypes on p.ProductTypeId equals pt.Id
                        where p.IsFrozen == false || p.IsFrozen == null
                        select new ProductInfo
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            Description = p.Description,
                            Discount = (decimal)p.Discount,
                            ManufacturerId = (int)p.ManufacturerId,
                            ManufacturerName = m.Name,
                            ProductTypeId = (int)p.ProductTypeId,
                            ProductTypeName = pt.Name,
                            Rating = (decimal)p.Rating,
                            IsFrozen = p.IsFrozen == true,
                            CreatedAt = (DateTime)p.CreatedAt
                        };

            _allProducts = query.ToList();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = _allProducts.AsEnumerable();

            string search = TxtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(p => p.Name.ToLower().Contains(search));
            }

            if (CmbProductType.SelectedValue != null && (int)CmbProductType.SelectedValue > 0)
            {
                filtered = filtered.Where(p => p.ProductTypeId == (int)CmbProductType.SelectedValue);
            }

            if (CmbManufacturer.SelectedValue != null && (int)CmbManufacturer.SelectedValue > 0)
            {
                filtered = filtered.Where(p => p.ManufacturerId == (int)CmbManufacturer.SelectedValue);
            }

            if (CmbSort.SelectedIndex == 0)
            {
                filtered = filtered.OrderByDescending(p => p.Rating);
            }
            else
            {
                filtered = filtered.OrderBy(p => p.Rating);
            }

            ItemsControlProducts.ItemsSource = filtered;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void CmbProductType_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void CmbManufacturer_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button)?.Tag as ProductInfo;
            if (product != null)
            {
                var detailWindow = new Window
                {
                    Title = product.Name,
                    Width = 450,
                    Height = 400,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = new ContentControl
                    {
                        Content = product,
                        ContentTemplate = (DataTemplate)Resources["ProductDetailTemplate"]
                    }
                };
                detailWindow.ShowDialog();
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUserId == 0)
            {
                var result = MessageBox.Show("Для добавления в корзину необходимо авторизоваться. Перейти на страницу входа?",
                    "Требуется авторизация", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    NavigationService.Navigate(new LoginPage());
                }
                return;
            }

            if (MainWindow.CurrentUserRole != "Клиент")
            {
                MessageBox.Show("Только клиенты могут покупать товары", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var product = (sender as Button)?.Tag as ProductInfo;
            if (product != null)
            {
                var existing = Core.Context.Carts.FirstOrDefault(c => c.UserId == MainWindow.CurrentUserId && c.ProductId == product.Id);
                if (existing != null)
                {
                    existing.Quantity++;
                }
                else
                {
                    Core.Context.Carts.Add(new Carts
                    {
                        UserId = MainWindow.CurrentUserId,
                        ProductId = product.Id,
                        Quantity = 1,
                        AddedAt = DateTime.Now
                    });
                }
                Core.Context.SaveChanges();
                MessageBox.Show($"{product.Name} добавлен в корзину по цене {product.FinalPrice} руб");
            }
        }

        private void BtnGoToCart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }

    public class ProductInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public decimal Discount { get; set; }
        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public decimal Rating { get; set; }
        public bool IsFrozen { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal FinalPrice => Price - (Price * Discount / 100);
        public bool HasDiscount => Discount > 0;
        public bool HasHighDiscount => Discount > 15;
    }
}
