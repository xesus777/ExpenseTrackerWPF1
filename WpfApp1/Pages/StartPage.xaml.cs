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
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
            LoadMastersServices();
        }

        private void LoadMastersServices()
        {
            var masters = Core.Context.Users.Where(u => u.RoleId == 2 && (u.IsFrozen == false || u.IsFrozen == null)).ToList();
            var masterServicesList = new List<MasterServiceInfo>();

            foreach (var master in masters)
            {
                var services = (from ms in Core.Context.MasterServices
                                join st in Core.Context.ServiceTypes on ms.ServiceTypeId equals st.Id
                                where ms.MasterId == master.Id
                                select new { st.Id, st.Name, st.Price, st.Duration }).ToList();

                if (services.Any())
                {
                    masterServicesList.Add(new MasterServiceInfo
                    {
                        MasterId = master.Id,
                        MasterName = master.FullName,
                        Services = services,
                        ServicesList = string.Join(", ", services.Select(s => s.Name)),
                        PricesList = string.Join(", ", services.Select(s => $"{s.Price} руб")),
                        DurationList = string.Join(", ", services.Select(s => $"{s.Duration} мин"))
                    });
                }
            }

            LvMastersServices.ItemsSource = masterServicesList;
        }

        private void LvMastersServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvMastersServices.SelectedItem != null)
            {
                var selected = (MasterServiceInfo)LvMastersServices.SelectedItem;

                if (selected.Services.Count == 1)
                {
                    var service = selected.Services.First();
                    ShowSlotsForService(selected.MasterId, service.Id, selected.MasterName, service.Name, service.Price);
                }
                else
                {
                    
                    ShowServiceSelection(selected);
                }
            }
        }

        private void ShowServiceSelection(MasterServiceInfo master)
        {
            var serviceWindow = new Window
            {
                Title = "Выбор услуги",
                Width = 400,
                Height = 350,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            stackPanel.Children.Add(new TextBlock
            {
                Text = $"Выберите услугу у мастера {master.MasterName}:",
                Margin = new Thickness(0, 0, 0, 10),
                FontWeight = FontWeights.Bold
            });

            var listBox = new ListBox { Height = 200, Margin = new Thickness(0, 0, 0, 10) };
            listBox.DisplayMemberPath = "Name";
            listBox.ItemsSource = master.Services;

            var button = new Button { Content = "Выбрать", Width = 100, Height = 30 };
            button.Click += (s, args) =>
            {
                if (listBox.SelectedItem != null)
                {
                    var selectedService = (dynamic)listBox.SelectedItem;
                    ShowSlotsForService(master.MasterId, selectedService.Id, master.MasterName, selectedService.Name, selectedService.Price);
                    serviceWindow.Close();
                }
                else
                {
                    MessageBox.Show("Выберите услугу");
                }
            };

            stackPanel.Children.Add(listBox);
            stackPanel.Children.Add(button);

            serviceWindow.Content = stackPanel;
            serviceWindow.ShowDialog();
        }

        private void ShowSlotsForService(int masterId, int serviceId, string masterName, string serviceName, decimal price)
        {
            if (MainWindow.CurrentUserId == 0)
            {
                var result = MessageBox.Show("Для записи необходимо авторизоваться. Перейти на страницу входа?",
                    "Требуется авторизация", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    NavigationService.Navigate(new LoginPage());
                }
                return;
            }

            if (MainWindow.CurrentUserRole != "Клиент")
            {
                MessageBox.Show("Только клиенты могут записываться на услуги", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var slotsWindow = new Window
            {
                Title = $"Запись к {masterName} - {serviceName}",
                Width = 500,
                Height = 450,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };

            var infoBorder = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.Gray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 10)
            };
            var infoStack = new StackPanel();
            infoStack.Children.Add(new TextBlock { Text = $"Мастер: {masterName}", FontWeight = FontWeights.Bold });
            infoStack.Children.Add(new TextBlock { Text = $"Услуга: {serviceName}", Margin = new Thickness(0, 5, 0, 0) });
            infoStack.Children.Add(new TextBlock { Text = $"Цена: {price} руб", Margin = new Thickness(0, 5, 0, 0) });
            infoBorder.Child = infoStack;
            stackPanel.Children.Add(infoBorder);

            stackPanel.Children.Add(new TextBlock { Text = "Выберите дату:", Margin = new Thickness(0, 0, 0, 5) });
            var datePicker = new DatePicker { Margin = new Thickness(0, 0, 0, 10) };
            datePicker.SelectedDate = DateTime.Now.AddDays(1);
            datePicker.DisplayDateStart = DateTime.Now.AddDays(1);
            stackPanel.Children.Add(datePicker);

            stackPanel.Children.Add(new TextBlock { Text = "Доступное время:", Margin = new Thickness(0, 10, 0, 5) });
            var slotsListBox = new ListBox { Height = 150, Margin = new Thickness(0, 0, 0, 10) };
            slotsListBox.DisplayMemberPath = "TimeDisplay";
            stackPanel.Children.Add(slotsListBox);

            var bookButton = new Button { Content = "Записаться", Width = 120, Height = 35, IsEnabled = false };
            bookButton.Click += (s, e) =>
            {
                if (slotsListBox.SelectedItem != null)
                {
                    var selected = (SlotDisplay)slotsListBox.SelectedItem;

                    var confirmResult = MessageBox.Show($"Подтвердите запись:\n\nМастер: {masterName}\nУслуга: {serviceName}\nДата и время: {selected.DateTime:dd.MM.yyyy HH:mm}\nЦена: {price} руб\n\nПродолжить?",
                        "Подтверждение записи", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (confirmResult == MessageBoxResult.Yes)
                    {
                        var appointment = new Appointments
                        {
                            ClientId = MainWindow.CurrentUserId,
                            MasterId = masterId,
                            ServiceTypeId = serviceId,
                            AppointmentDateTime = selected.DateTime,
                            Price = price,
                            PaymentMethod = null,
                            Comment = null,
                            Status = "Scheduled",
                            CreatedAt = DateTime.Now
                        };

                        Core.Context.Appointments.Add(appointment);
                        Core.Context.SaveChanges();

                        MessageBox.Show("Вы успешно записаны!");
                        slotsWindow.Close();
                    }
                }
            };
            stackPanel.Children.Add(bookButton);

            datePicker.SelectedDateChanged += (s, e) =>
            {
                if (datePicker.SelectedDate != null)
                {
                    LoadSlots(masterId, datePicker.SelectedDate.Value, slotsListBox, bookButton);
                }
            };

            slotsListBox.SelectionChanged += (s, e) => bookButton.IsEnabled = slotsListBox.SelectedItem != null;

            if (datePicker.SelectedDate != null)
            {
                LoadSlots(masterId, datePicker.SelectedDate.Value, slotsListBox, bookButton);
            }

            slotsWindow.Content = stackPanel;
            slotsWindow.ShowDialog();
        }

        private void LoadSlots(int masterId, DateTime selectedDate, ListBox listBox, Button bookButton)
        {
            var existingAppointments = Core.Context.Appointments
                .Where(a => a.MasterId == masterId && a.Status != "Cancelled")
                .Select(a => a.AppointmentDateTime)
                .ToList();

            var slots = new List<SlotDisplay>();

            for (int hour = 10; hour <= 18; hour++)
            {
                var slotTime = selectedDate.Date.AddHours(hour);
                if (!existingAppointments.Contains(slotTime) && slotTime > DateTime.Now)
                {
                    slots.Add(new SlotDisplay
                    {
                        DateTime = slotTime,
                        TimeDisplay = slotTime.ToString("HH:mm")
                    });
                }
            }

            listBox.ItemsSource = slots;
            bookButton.IsEnabled = false;

            if (!slots.Any())
            {
                listBox.ItemsSource = new List<SlotDisplay> { new SlotDisplay { TimeDisplay = "Нет свободного времени на эту дату" } };
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }

    public class MasterServiceInfo
    {
        public int MasterId { get; set; }
        public string MasterName { get; set; }
        public dynamic Services { get; set; }
        public string ServicesList { get; set; }
        public string PricesList { get; set; }
        public string DurationList { get; set; }
    }

    public class SlotDisplay
    {
        public DateTime DateTime { get; set; }
        public string TimeDisplay { get; set; }
    }

    public class BookingInfo
    {
        public int ServiceId { get; set; }
        public int MasterId { get; set; }
        public string ServiceName { get; set; }
        public string MasterName { get; set; }
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
    }
}
