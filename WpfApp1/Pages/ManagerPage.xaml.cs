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
    public partial class ManagerPage : Page
    {
        private List<AppointmentView> _allAppointments;

        public ManagerPage()
        {
            InitializeComponent();
            LoadAppointments();
            LoadOrders();
            LoadProducts();
            LoadManufacturers();
            LoadProductTypes();
            LoadServiceTypes();
        }

        private void LoadAppointments()
        {
            _allAppointments = (from a in Core.Context.Appointments
                                join c in Core.Context.Users on a.ClientId equals c.Id
                                join m in Core.Context.Users on a.MasterId equals m.Id
                                join st in Core.Context.ServiceTypes on a.ServiceTypeId equals st.Id
                                select new AppointmentView
                                {
                                    Id = a.Id,
                                    AppointmentDateTime = a.AppointmentDateTime,
                                    ClientName = c.FullName,
                                    MasterName = m.FullName,
                                    ServiceName = st.Name,
                                    Status = a.Status,
                                    ClientId = (int)a.ClientId,
                                    MasterId = (int)a.MasterId,
                                    ServiceTypeId = (int)a.ServiceTypeId,
                                    Price = a.Price,
                                    PaymentMethod = a.PaymentMethod,
                                    Comment = a.Comment
                                }).ToList();
            LvAppointments.ItemsSource = _allAppointments;
        }

        private void TxtSearchClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            var search = TxtSearchClient.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(search))
                LvAppointments.ItemsSource = _allAppointments;
            else
                LvAppointments.ItemsSource = _allAppointments.Where(a => a.ClientName.ToLower().Contains(search)).ToList();
        }

        private void CreateAppointment_Click(object sender, RoutedEventArgs e)
        {
            var clients = Core.Context.Users.Where(u => u.RoleId == 1 && (u.IsFrozen == false || u.IsFrozen == null)).ToList();
            var masters = Core.Context.Users.Where(u => u.RoleId == 2 && (u.IsFrozen == false || u.IsFrozen == null)).ToList();
            var services = Core.Context.ServiceTypes.ToList();

            var createWindow = new Window
            {
                Title = "Создание записи",
                Width = 400,
                Height = 450,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };

            stackPanel.Children.Add(new TextBlock { Text = "Клиент:" });
            var clientCombo = new ComboBox { DisplayMemberPath = "FullName", SelectedValuePath = "Id", Height = 25, Margin = new Thickness(0, 5, 0, 10) };
            clientCombo.ItemsSource = clients;
            stackPanel.Children.Add(clientCombo);

            stackPanel.Children.Add(new TextBlock { Text = "Мастер:" });
            var masterCombo = new ComboBox { DisplayMemberPath = "FullName", SelectedValuePath = "Id", Height = 25, Margin = new Thickness(0, 5, 0, 10) };
            masterCombo.ItemsSource = masters;
            stackPanel.Children.Add(masterCombo);

            stackPanel.Children.Add(new TextBlock { Text = "Услуга:" });
            var serviceCombo = new ComboBox { DisplayMemberPath = "Name", SelectedValuePath = "Id", Height = 25, Margin = new Thickness(0, 5, 0, 10) };
            serviceCombo.ItemsSource = services;
            stackPanel.Children.Add(serviceCombo);

            stackPanel.Children.Add(new TextBlock { Text = "Дата и время:" });
            var datePicker = new DatePicker { Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(datePicker);

            var hourCombo = new ComboBox { Height = 25, Margin = new Thickness(0, 5, 0, 10) };
            for (int h = 10; h <= 18; h++) hourCombo.Items.Add($"{h}:00");
            hourCombo.SelectedIndex = 0;
            stackPanel.Children.Add(hourCombo);

            stackPanel.Children.Add(new TextBlock { Text = "Способ оплаты:" });
            var paymentCombo = new ComboBox { Height = 25, Margin = new Thickness(0, 5, 0, 10) };
            paymentCombo.Items.Add("Наличные");
            paymentCombo.Items.Add("Карта");
            paymentCombo.Items.Add("Онлайн");
            paymentCombo.SelectedIndex = 0;
            stackPanel.Children.Add(paymentCombo);

            stackPanel.Children.Add(new TextBlock { Text = "Комментарий:" });
            var commentBox = new TextBox { Height = 60, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(commentBox);

            var saveBtn = new Button { Content = "Создать", Width = 100, Height = 30 };
            saveBtn.Click += (s, args) =>
            {
                if (clientCombo.SelectedValue == null || masterCombo.SelectedValue == null || serviceCombo.SelectedValue == null || datePicker.SelectedDate == null)
                {
                    MessageBox.Show("Заполните все поля");
                    return;
                }

                var dateTime = datePicker.SelectedDate.Value.Date.AddHours(int.Parse(hourCombo.SelectedItem.ToString().Split(':')[0]));
                var service = Core.Context.ServiceTypes.FirstOrDefault(st => st.Id == (int)serviceCombo.SelectedValue);

                var appointment = new Appointments
                {
                    ClientId = (int)clientCombo.SelectedValue,
                    MasterId = (int)masterCombo.SelectedValue,
                    ServiceTypeId = (int)serviceCombo.SelectedValue,
                    AppointmentDateTime = dateTime,
                    Price = service.Price,
                    PaymentMethod = paymentCombo.SelectedItem.ToString(),
                    Comment = commentBox.Text,
                    Status = "Scheduled",
                    CreatedAt = DateTime.Now
                };

                Core.Context.Appointments.Add(appointment);
                Core.Context.SaveChanges();
                MessageBox.Show("Запись создана");
                LoadAppointments();
                createWindow.Close();
            };

            stackPanel.Children.Add(saveBtn);
            createWindow.Content = stackPanel;
            createWindow.ShowDialog();
        }

        private void LvAppointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvAppointments.SelectedItem != null)
            {
                var selected = (AppointmentView)LvAppointments.SelectedItem;
                var appointment = Core.Context.Appointments.FirstOrDefault(a => a.Id == selected.Id);

                if (appointment != null)
                {
                    var actionWindow = new Window
                    {
                        Title = "Действие с записью",
                        Width = 300,
                        Height = 200,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    var stackPanel = new StackPanel { Margin = new Thickness(10) };
                    stackPanel.Children.Add(new TextBlock { Text = $"Запись от {appointment.AppointmentDateTime:dd.MM.yyyy HH:mm}" });

                    var newDatePicker = new DatePicker { Margin = new Thickness(0, 10, 0, 5) };
                    stackPanel.Children.Add(newDatePicker);

                    var newHourCombo = new ComboBox { Margin = new Thickness(0, 0, 0, 10) };
                    for (int h = 10; h <= 18; h++) newHourCombo.Items.Add($"{h}:00");
                    newHourCombo.SelectedIndex = 0;
                    stackPanel.Children.Add(newHourCombo);

                    var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
                    var rescheduleBtn = new Button { Content = "Перенести", Width = 80, Margin = new Thickness(5) };
                    var cancelBtn = new Button { Content = "Отменить", Width = 80, Margin = new Thickness(5) };
                    var closeBtn = new Button { Content = "Закрыть", Width = 80, Margin = new Thickness(5) };

                    rescheduleBtn.Click += (s, args) =>
                    {
                        if (newDatePicker.SelectedDate != null)
                        {
                            var newDateTime = newDatePicker.SelectedDate.Value.Date.AddHours(int.Parse(newHourCombo.SelectedItem.ToString().Split(':')[0]));
                            appointment.AppointmentDateTime = newDateTime;
                            Core.Context.SaveChanges();
                            MessageBox.Show("Запись перенесена");
                            LoadAppointments();
                            actionWindow.Close();
                        }
                    };

                    cancelBtn.Click += (s, args) =>
                    {
                        appointment.Status = "Cancelled";
                        Core.Context.SaveChanges();
                        MessageBox.Show("Запись отменена");
                        LoadAppointments();
                        actionWindow.Close();
                    };

                    closeBtn.Click += (s, args) => actionWindow.Close();

                    buttonPanel.Children.Add(rescheduleBtn);
                    buttonPanel.Children.Add(cancelBtn);
                    buttonPanel.Children.Add(closeBtn);
                    stackPanel.Children.Add(buttonPanel);

                    actionWindow.Content = stackPanel;
                    actionWindow.ShowDialog();
                }
            }
        }

        private void LoadOrders()
        {
            var orders = (from o in Core.Context.Orders
                          join u in Core.Context.Users on o.UserId equals u.Id
                          select new OrderView
                          {
                              Id = o.Id,
                              ClientName = u.FullName,
                              OrderDate = (DateTime)o.OrderDate,
                              DeliveryDate = o.DeliveryDate,
                              TotalAmount = o.TotalAmount,
                              Status = o.Status
                          }).ToList();
            LvOrders.ItemsSource = orders;
        }

        private void LvOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvOrders.SelectedItem != null)
            {
                var selected = (OrderView)LvOrders.SelectedItem;
                var order = Core.Context.Orders.FirstOrDefault(o => o.Id == selected.Id);

                if (order != null && order.Status == "Pending")
                {
                    var result = MessageBox.Show($"Выдать заказ №{order.Id}?", "Выдача заказа", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        order.Status = "Completed";
                        Core.Context.SaveChanges();
                        LoadOrders();
                        MessageBox.Show("Заказ выдан");
                    }
                }
                else if (order != null && order.Status == "Completed")
                {
                    MessageBox.Show("Заказ уже выдан");
                }
            }
        }

        private void LoadProducts()
        {
            var products = (from p in Core.Context.Products
                            join m in Core.Context.Manufacturers on p.ManufacturerId equals m.Id
                            join pt in Core.Context.ProductTypes on p.ProductTypeId equals pt.Id
                            select new ProductView
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Price = p.Price,
                                Discount = (decimal)p.Discount,
                                IsFrozen = p.IsFrozen == true,
                                ManufacturerName = m.Name,
                                ProductTypeName = pt.Name
                            }).ToList();
            LvProducts.ItemsSource = products;
        }

        private void LvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvProducts.SelectedItem != null)
            {
                var selected = (ProductView)LvProducts.SelectedItem;
                var product = Core.Context.Products.FirstOrDefault(p => p.Id == selected.Id);

                if (product != null)
                {
                    var editWindow = new Window
                    {
                        Title = "Редактирование товара",
                        Width = 400,
                        Height = 450,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    var stackPanel = new StackPanel { Margin = new Thickness(10) };

                    stackPanel.Children.Add(new TextBlock { Text = "Название:" });
                    var nameBox = new TextBox { Text = product.Name, Margin = new Thickness(0, 5, 0, 10) };
                    stackPanel.Children.Add(nameBox);

                    stackPanel.Children.Add(new TextBlock { Text = "Цена:" });
                    var priceBox = new TextBox { Text = product.Price.ToString(), Margin = new Thickness(0, 5, 0, 10) };
                    stackPanel.Children.Add(priceBox);

                    stackPanel.Children.Add(new TextBlock { Text = "Скидка (%):" });
                    var discountBox = new TextBox { Text = product.Discount.ToString(), Margin = new Thickness(0, 5, 0, 10) };
                    stackPanel.Children.Add(discountBox);

                    stackPanel.Children.Add(new TextBlock { Text = "Описание:" });
                    var descBox = new TextBox { Text = product.Description, Height = 60, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 5, 0, 10) };
                    stackPanel.Children.Add(descBox);

                    stackPanel.Children.Add(new TextBlock { Text = "Рейтинг:" });
                    var ratingBox = new TextBox { Text = product.Rating.ToString(), Margin = new Thickness(0, 5, 0, 10) };
                    stackPanel.Children.Add(ratingBox);

                    var frozenCheck = new CheckBox { Content = "Заморожен (не продается)", IsChecked = product.IsFrozen == true };
                    stackPanel.Children.Add(frozenCheck);

                    var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
                    var saveBtn = new Button { Content = "Сохранить", Width = 80, Margin = new Thickness(5) };
                    var deleteBtn = new Button { Content = "Удалить", Width = 80, Margin = new Thickness(5) };

                    saveBtn.Click += (s, args) =>
                    {
                        product.Name = nameBox.Text;
                        if (decimal.TryParse(priceBox.Text, out decimal price)) product.Price = price;
                        if (decimal.TryParse(discountBox.Text, out decimal discount)) product.Discount = discount;
                        product.Description = descBox.Text;
                        if (decimal.TryParse(ratingBox.Text, out decimal rating)) product.Rating = rating;
                        product.IsFrozen = frozenCheck.IsChecked;
                        Core.Context.SaveChanges();
                        LoadProducts();
                        MessageBox.Show("Товар обновлен");
                        editWindow.Close();
                    };

                    deleteBtn.Click += (s, args) =>
                    {
                        if (MessageBox.Show("Удалить товар?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Core.Context.Products.Remove(product);
                            Core.Context.SaveChanges();
                            LoadProducts();
                            MessageBox.Show("Товар удален");
                            editWindow.Close();
                        }
                    };

                    buttonPanel.Children.Add(saveBtn);
                    buttonPanel.Children.Add(deleteBtn);
                    stackPanel.Children.Add(buttonPanel);

                    editWindow.Content = stackPanel;
                    editWindow.ShowDialog();
                }
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var manufacturers = Core.Context.Manufacturers.ToList();
            var productTypes = Core.Context.ProductTypes.ToList();

            var addWindow = new Window
            {
                Title = "Добавление товара",
                Width = 400,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };

            stackPanel.Children.Add(new TextBlock { Text = "Название:" });
            var nameBox = new TextBox { Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(nameBox);

            stackPanel.Children.Add(new TextBlock { Text = "Цена:" });
            var priceBox = new TextBox { Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(priceBox);

            stackPanel.Children.Add(new TextBlock { Text = "Скидка (%):" });
            var discountBox = new TextBox { Text = "0", Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(discountBox);

            stackPanel.Children.Add(new TextBlock { Text = "Описание:" });
            var descBox = new TextBox { Height = 60, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(descBox);

            stackPanel.Children.Add(new TextBlock { Text = "Производитель:" });
            var manCombo = new ComboBox { DisplayMemberPath = "Name", SelectedValuePath = "Id", Margin = new Thickness(0, 5, 0, 10) };
            manCombo.ItemsSource = manufacturers;
            stackPanel.Children.Add(manCombo);

            stackPanel.Children.Add(new TextBlock { Text = "Тип товара:" });
            var typeCombo = new ComboBox { DisplayMemberPath = "Name", SelectedValuePath = "Id", Margin = new Thickness(0, 5, 0, 10) };
            typeCombo.ItemsSource = productTypes;
            stackPanel.Children.Add(typeCombo);

            stackPanel.Children.Add(new TextBlock { Text = "Рейтинг:" });
            var ratingBox = new TextBox { Text = "0", Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(ratingBox);

            var saveBtn = new Button { Content = "Добавить", Width = 100, Height = 30 };
            saveBtn.Click += (s, args) =>
            {
                if (string.IsNullOrEmpty(nameBox.Text) || manCombo.SelectedValue == null || typeCombo.SelectedValue == null)
                {
                    MessageBox.Show("Заполните обязательные поля");
                    return;
                }

                var product = new Products
                {
                    Name = nameBox.Text,
                    Price = decimal.TryParse(priceBox.Text, out decimal price) ? price : 0,
                    Discount = decimal.TryParse(discountBox.Text, out decimal discount) ? discount : 0,
                    Description = descBox.Text,
                    ManufacturerId = (int)manCombo.SelectedValue,
                    ProductTypeId = (int)typeCombo.SelectedValue,
                    Rating = decimal.TryParse(ratingBox.Text, out decimal rating) ? rating : 0,
                    IsFrozen = false,
                    CreatedAt = DateTime.Now
                };

                Core.Context.Products.Add(product);
                Core.Context.SaveChanges();
                LoadProducts();
                MessageBox.Show("Товар добавлен");
                addWindow.Close();
            };

            stackPanel.Children.Add(saveBtn);
            addWindow.Content = stackPanel;
            addWindow.ShowDialog();
        }

        private void LoadManufacturers()
        {
            LvManufacturers.ItemsSource = Core.Context.Manufacturers.ToList();
        }

        private void LvManufacturers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvManufacturers.SelectedItem != null)
            {
                var manufacturer = (Manufacturers)LvManufacturers.SelectedItem;
                var editWindow = new Window
                {
                    Title = "Редактирование производителя",
                    Width = 300,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                var stackPanel = new StackPanel { Margin = new Thickness(10) };
                stackPanel.Children.Add(new TextBlock { Text = "Название:" });
                var nameBox = new TextBox { Text = manufacturer.Name, Margin = new Thickness(0, 5, 0, 10) };
                stackPanel.Children.Add(nameBox);

                var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };
                var saveBtn = new Button { Content = "Сохранить", Width = 80, Margin = new Thickness(5) };
                var deleteBtn = new Button { Content = "Удалить", Width = 80, Margin = new Thickness(5) };

                saveBtn.Click += (s, args) =>
                {
                    manufacturer.Name = nameBox.Text;
                    Core.Context.SaveChanges();
                    LoadManufacturers();
                    editWindow.Close();
                };

                deleteBtn.Click += (s, args) =>
                {
                    if (MessageBox.Show("Удалить?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Core.Context.Manufacturers.Remove(manufacturer);
                        Core.Context.SaveChanges();
                        LoadManufacturers();
                        editWindow.Close();
                    }
                };

                buttonPanel.Children.Add(saveBtn);
                buttonPanel.Children.Add(deleteBtn);
                stackPanel.Children.Add(buttonPanel);
                editWindow.Content = stackPanel;
                editWindow.ShowDialog();
            }
        }

        private void AddManufacturer_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new Window
            {
                Title = "Добавление производителя",
                Width = 300,
                Height = 120,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            stackPanel.Children.Add(new TextBlock { Text = "Название:" });
            var nameBox = new TextBox { Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(nameBox);

            var saveBtn = new Button { Content = "Добавить", Width = 100, Height = 30 };
            saveBtn.Click += (s, args) =>
            {
                if (!string.IsNullOrEmpty(nameBox.Text))
                {
                    Core.Context.Manufacturers.Add(new Manufacturers { Name = nameBox.Text });
                    Core.Context.SaveChanges();
                    LoadManufacturers();
                    addWindow.Close();
                }
            };
            stackPanel.Children.Add(saveBtn);
            addWindow.Content = stackPanel;
            addWindow.ShowDialog();
        }

        private void LoadProductTypes()
        {
            LvProductTypes.ItemsSource = Core.Context.ProductTypes.ToList();
        }

        private void LvProductTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvProductTypes.SelectedItem != null)
            {
                var productType = (ProductTypes)LvProductTypes.SelectedItem;
                var editWindow = new Window
                {
                    Title = "Редактирование типа товара",
                    Width = 300,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                var stackPanel = new StackPanel { Margin = new Thickness(10) };
                stackPanel.Children.Add(new TextBlock { Text = "Название:" });
                var nameBox = new TextBox { Text = productType.Name, Margin = new Thickness(0, 5, 0, 10) };
                stackPanel.Children.Add(nameBox);

                var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };
                var saveBtn = new Button { Content = "Сохранить", Width = 80, Margin = new Thickness(5) };
                var deleteBtn = new Button { Content = "Удалить", Width = 80, Margin = new Thickness(5) };

                saveBtn.Click += (s, args) =>
                {
                    productType.Name = nameBox.Text;
                    Core.Context.SaveChanges();
                    LoadProductTypes();
                    editWindow.Close();
                };

                deleteBtn.Click += (s, args) =>
                {
                    if (MessageBox.Show("Удалить?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Core.Context.ProductTypes.Remove(productType);
                        Core.Context.SaveChanges();
                        LoadProductTypes();
                        editWindow.Close();
                    }
                };

                buttonPanel.Children.Add(saveBtn);
                buttonPanel.Children.Add(deleteBtn);
                stackPanel.Children.Add(buttonPanel);
                editWindow.Content = stackPanel;
                editWindow.ShowDialog();
            }
        }

        private void AddProductType_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new Window
            {
                Title = "Добавление типа товара",
                Width = 300,
                Height = 120,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            stackPanel.Children.Add(new TextBlock { Text = "Название:" });
            var nameBox = new TextBox { Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(nameBox);

            var saveBtn = new Button { Content = "Добавить", Width = 100, Height = 30 };
            saveBtn.Click += (s, args) =>
            {
                if (!string.IsNullOrEmpty(nameBox.Text))
                {
                    Core.Context.ProductTypes.Add(new ProductTypes { Name = nameBox.Text });
                    Core.Context.SaveChanges();
                    LoadProductTypes();
                    addWindow.Close();
                }
            };
            stackPanel.Children.Add(saveBtn);
            addWindow.Content = stackPanel;
            addWindow.ShowDialog();
        }

        private void LoadServiceTypes()
        {
            LvServiceTypes.ItemsSource = Core.Context.ServiceTypes.ToList();
        }

        private void LvServiceTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvServiceTypes.SelectedItem != null)
            {
                var serviceType = (ServiceTypes)LvServiceTypes.SelectedItem;
                var editWindow = new Window
                {
                    Title = "Редактирование услуги",
                    Width = 350,
                    Height = 250,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                var stackPanel = new StackPanel { Margin = new Thickness(10) };
                stackPanel.Children.Add(new TextBlock { Text = "Название:" });
                var nameBox = new TextBox { Text = serviceType.Name, Margin = new Thickness(0, 5, 0, 10) };
                stackPanel.Children.Add(nameBox);

                stackPanel.Children.Add(new TextBlock { Text = "Цена:" });
                var priceBox = new TextBox { Text = serviceType.Price.ToString(), Margin = new Thickness(0, 5, 0, 10) };
                stackPanel.Children.Add(priceBox);

                stackPanel.Children.Add(new TextBlock { Text = "Длительность (мин):" });
                var durationBox = new TextBox { Text = serviceType.Duration.ToString(), Margin = new Thickness(0, 5, 0, 10) };
                stackPanel.Children.Add(durationBox);

                var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };
                var saveBtn = new Button { Content = "Сохранить", Width = 80, Margin = new Thickness(5) };
                var deleteBtn = new Button { Content = "Удалить", Width = 80, Margin = new Thickness(5) };

                saveBtn.Click += (s, args) =>
                {
                    serviceType.Name = nameBox.Text;
                    if (decimal.TryParse(priceBox.Text, out decimal price)) serviceType.Price = price;
                    if (int.TryParse(durationBox.Text, out int duration)) serviceType.Duration = duration;
                    Core.Context.SaveChanges();
                    LoadServiceTypes();
                    editWindow.Close();
                };

                deleteBtn.Click += (s, args) =>
                {
                    if (MessageBox.Show("Удалить?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Core.Context.ServiceTypes.Remove(serviceType);
                        Core.Context.SaveChanges();
                        LoadServiceTypes();
                        editWindow.Close();
                    }
                };

                buttonPanel.Children.Add(saveBtn);
                buttonPanel.Children.Add(deleteBtn);
                stackPanel.Children.Add(buttonPanel);
                editWindow.Content = stackPanel;
                editWindow.ShowDialog();
            }
        }

        private void AddServiceType_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new Window
            {
                Title = "Добавление услуги",
                Width = 350,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            stackPanel.Children.Add(new TextBlock { Text = "Название:" });
            var nameBox = new TextBox { Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(nameBox);

            stackPanel.Children.Add(new TextBlock { Text = "Цена:" });
            var priceBox = new TextBox { Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(priceBox);

            stackPanel.Children.Add(new TextBlock { Text = "Длительность (мин):" });
            var durationBox = new TextBox { Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(durationBox);

            var saveBtn = new Button { Content = "Добавить", Width = 100, Height = 30 };
            saveBtn.Click += (s, args) =>
            {
                if (!string.IsNullOrEmpty(nameBox.Text))
                {
                    Core.Context.ServiceTypes.Add(new ServiceTypes
                    {
                        Name = nameBox.Text,
                        Price = decimal.TryParse(priceBox.Text, out decimal price) ? price : 0,
                        Duration = int.TryParse(durationBox.Text, out int duration) ? duration : 0
                    });
                    Core.Context.SaveChanges();
                    LoadServiceTypes();
                    addWindow.Close();
                }
            };
            stackPanel.Children.Add(saveBtn);
            addWindow.Content = stackPanel;
            addWindow.ShowDialog();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }

    public class AppointmentView
    {
        public int Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string ClientName { get; set; }
        public string MasterName { get; set; }
        public string ServiceName { get; set; }
        public string Status { get; set; }
        public int ClientId { get; set; }
        public int MasterId { get; set; }
        public int ServiceTypeId { get; set; }
        public decimal Price { get; set; }
        public string PaymentMethod { get; set; }
        public string Comment { get; set; }
    }

    public class OrderView
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }

    public class ProductView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public bool IsFrozen { get; set; }
        public string ManufacturerName { get; set; }
        public string ProductTypeName { get; set; }
    }
}
