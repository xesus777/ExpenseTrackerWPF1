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
    public partial class MasterPage : Page
    {
        public MasterPage()
        {
            InitializeComponent();
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            var appointments = from a in Core.Context.Appointments
                               join st in Core.Context.ServiceTypes on a.ServiceTypeId equals st.Id
                               join c in Core.Context.Users on a.ClientId equals c.Id
                               where a.MasterId == MainWindow.CurrentUserId
                               orderby a.AppointmentDateTime descending
                               select new
                               {
                                   a.Id,
                                   a.AppointmentDateTime,
                                   ServiceName = st.Name,
                                   ClientName = c.FullName,
                                   ClientPhone = c.Phone,
                                   a.Status
                               };
            LvAppointments.ItemsSource = appointments.ToList();
        }

        private void LvAppointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvAppointments.SelectedItem != null)
            {
                var selected = LvAppointments.SelectedItem;
                var idProperty = selected.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    int appointmentId = (int)idProperty.GetValue(selected);
                    NavigationService.Navigate(new AppointmentDetailsPage(appointmentId));
                    LvAppointments.SelectedItem = null;
                }
            }
        }



        private void EditServices_Click(object sender, RoutedEventArgs e)
        {
            var servicesWindow = new Window
            {
                Title = "Мои услуги",
                Width = 400,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var allServices = Core.Context.ServiceTypes.ToList();
            var myServices = Core.Context.MasterServices
                .Where(ms => ms.MasterId == MainWindow.CurrentUserId)
                .Select(ms => ms.ServiceTypeId)
                .ToList();

            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            stackPanel.Children.Add(new TextBlock { Text = "Выберите услуги, которые вы оказываете:", Margin = new Thickness(0, 0, 0, 10) });

            var listBox = new ListBox { SelectionMode = SelectionMode.Multiple, Height = 250 };
            listBox.DisplayMemberPath = "Name";
            listBox.ItemsSource = allServices;

            for (int i = 0; i < allServices.Count; i++)
            {
                if (myServices.Contains(allServices[i].Id))
                {
                    listBox.SelectedItems.Add(allServices[i]);
                }
            }

            stackPanel.Children.Add(listBox);

            var saveBtn = new Button { Content = "Сохранить", Width = 100, Height = 30, Margin = new Thickness(0, 10, 0, 0) };
            saveBtn.Click += (s, args) =>
            {
                var selectedIds = listBox.SelectedItems.Cast<ServiceTypes>().Select(st => st.Id).ToList();

                var toRemove = Core.Context.MasterServices.Where(ms => ms.MasterId == MainWindow.CurrentUserId && !selectedIds.Contains((int)ms.ServiceTypeId));
                Core.Context.MasterServices.RemoveRange(toRemove);

                foreach (var serviceId in selectedIds)
                {
                    if (!Core.Context.MasterServices.Any(ms => ms.MasterId == MainWindow.CurrentUserId && ms.ServiceTypeId == serviceId))
                    {
                        Core.Context.MasterServices.Add(new MasterServices
                        {
                            MasterId = MainWindow.CurrentUserId,
                            ServiceTypeId = serviceId
                        });
                    }
                }

                Core.Context.SaveChanges();
                MessageBox.Show("Услуги обновлены");
                servicesWindow.Close();
            };

            stackPanel.Children.Add(saveBtn);
            servicesWindow.Content = stackPanel;
            servicesWindow.ShowDialog();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
