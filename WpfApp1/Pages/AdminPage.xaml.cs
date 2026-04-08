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
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            var users = from u in Core.Context.Users
                        join r in Core.Context.Roles on u.RoleId equals r.Id
                        select new UserView
                        {
                            Id = u.Id,
                            FullName = u.FullName,
                            Phone = u.Phone,
                            Email = u.Email,
                            Login = u.Login,
                            RoleName = r.Name,
                            IsFrozen = u.IsFrozen == true
                        };
            LvUsers.ItemsSource = users.ToList();
        }

        private void LvUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LvUsers.SelectedItem != null)
            {
                var selected = (UserView)LvUsers.SelectedItem;
                var user = Core.Context.Users.FirstOrDefault(u => u.Id == selected.Id);
                var roles = Core.Context.Roles.ToList();

                if (user != null)
                {
                    var editWindow = new Window
                    {
                        Title = "Редактирование пользователя",
                        Width = 400,
                        Height = 430,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    var stackPanel = new StackPanel { Margin = new Thickness(10) };

                    stackPanel.Children.Add(new TextBlock { Text = "ФИО:" });
                    var nameBox = new TextBox { Text = user.FullName, Margin = new Thickness(0, 5, 0, 10) };
                    stackPanel.Children.Add(nameBox);

                    stackPanel.Children.Add(new TextBlock { Text = "Телефон:" });
                    var phoneBox = new TextBox { Text = user.Phone, Margin = new Thickness(0, 5, 0, 10) };
                    stackPanel.Children.Add(phoneBox);

                    stackPanel.Children.Add(new TextBlock { Text = "Email:" });
                    var emailBox = new TextBox { Text = user.Email, Margin = new Thickness(0, 5, 0, 10) };
                    stackPanel.Children.Add(emailBox);

                    stackPanel.Children.Add(new TextBlock { Text = "Логин:" });
                    var loginBox = new TextBox { Text = user.Login, Margin = new Thickness(0, 5, 0, 10) };
                    stackPanel.Children.Add(loginBox);

                    stackPanel.Children.Add(new TextBlock { Text = "Пароль:" });
                    var passBox = new PasswordBox { Margin = new Thickness(0, 5, 0, 10) };
                    stackPanel.Children.Add(passBox);

                    stackPanel.Children.Add(new TextBlock { Text = "Роль:" });
                    var roleCombo = new ComboBox { DisplayMemberPath = "Name", SelectedValuePath = "Id", Margin = new Thickness(0, 5, 0, 10) };
                    roleCombo.ItemsSource = roles;
                    roleCombo.SelectedValue = user.RoleId;
                    stackPanel.Children.Add(roleCombo);

                    // Информация о статусе
                    var statusText = new TextBlock
                    {
                        Text = user.IsFrozen == true ? "Статус: ЗАМОРОЖЕН" : "Статус: АКТИВЕН",
                        Foreground = user.IsFrozen == true ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Green,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 10, 0, 5)
                    };
                    stackPanel.Children.Add(statusText);

                    var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 10, 0, 0) };
                    var saveBtn = new Button { Content = "Сохранить", Width = 80, Margin = new Thickness(5) };
                    var freezeBtn = new Button { Width = 100, Margin = new Thickness(5) };

                    if (user.IsFrozen == true)
                    {
                        freezeBtn.Content = "Разморозить";
                        freezeBtn.Background = System.Windows.Media.Brushes.LightGreen;
                    }
                    else
                    {
                        freezeBtn.Content = "Заморозить";
                        freezeBtn.Background = System.Windows.Media.Brushes.LightCoral;
                    }

                    saveBtn.Click += (s, args) =>
                    {
                        user.FullName = nameBox.Text;
                        user.Phone = phoneBox.Text;
                        user.Email = emailBox.Text;
                        user.Login = loginBox.Text;
                        if (!string.IsNullOrEmpty(passBox.Password))
                            user.Password = passBox.Password;
                        user.RoleId = (int)roleCombo.SelectedValue;
                        Core.Context.SaveChanges();
                        LoadUsers();
                        MessageBox.Show("Пользователь обновлен");
                        editWindow.Close();
                    };

                    freezeBtn.Click += (s, args) =>
                    {
                        if (user.Id == MainWindow.CurrentUserId)
                        {
                            MessageBox.Show("Нельзя заморозить/разморозить самого себя");
                            return;
                        }

                        user.IsFrozen = !user.IsFrozen;
                        Core.Context.SaveChanges();
                        LoadUsers();
                        MessageBox.Show(user.IsFrozen == true ? "Пользователь заморожен" : "Пользователь разморожен");
                        editWindow.Close();
                    };

                    buttonPanel.Children.Add(saveBtn);
                    buttonPanel.Children.Add(freezeBtn);
                    stackPanel.Children.Add(buttonPanel);

                    editWindow.Content = stackPanel;
                    editWindow.ShowDialog();
                }
            }
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var roles = Core.Context.Roles.ToList();

            var addWindow = new Window
            {
                Title = "Добавление пользователя",
                Width = 400,
                Height = 450,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };

            stackPanel.Children.Add(new TextBlock { Text = "ФИО:" });
            var nameBox = new TextBox { Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(nameBox);

            stackPanel.Children.Add(new TextBlock { Text = "Телефон:" });
            var phoneBox = new TextBox { Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(phoneBox);

            stackPanel.Children.Add(new TextBlock { Text = "Email:" });
            var emailBox = new TextBox { Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(emailBox);

            stackPanel.Children.Add(new TextBlock { Text = "Логин:" });
            var loginBox = new TextBox { Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(loginBox);

            stackPanel.Children.Add(new TextBlock { Text = "Пароль:" });
            var passBox = new PasswordBox { Margin = new Thickness(0, 5, 0, 10) };
            stackPanel.Children.Add(passBox);

            stackPanel.Children.Add(new TextBlock { Text = "Роль:" });
            var roleCombo = new ComboBox { DisplayMemberPath = "Name", SelectedValuePath = "Id", Margin = new Thickness(0, 5, 0, 10) };
            roleCombo.ItemsSource = roles;
            roleCombo.SelectedIndex = 0;
            stackPanel.Children.Add(roleCombo);

            var saveBtn = new Button { Content = "Добавить", Width = 100, Height = 30, Margin = new Thickness(0, 20, 0, 0) };
            saveBtn.Click += (s, args) =>
            {
                if (string.IsNullOrEmpty(nameBox.Text) || string.IsNullOrEmpty(loginBox.Text) || string.IsNullOrEmpty(passBox.Password))
                {
                    MessageBox.Show("Заполните обязательные поля");
                    return;
                }

                if (Core.Context.Users.Any(u => u.Login == loginBox.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует");
                    return;
                }

                var user = new Users
                {
                    FullName = nameBox.Text,
                    Phone = phoneBox.Text,
                    Email = emailBox.Text,
                    Login = loginBox.Text,
                    Password = passBox.Password,
                    RoleId = (int)roleCombo.SelectedValue,
                    IsFrozen = false,
                    CreatedAt = DateTime.Now
                };

                Core.Context.Users.Add(user);
                Core.Context.SaveChanges();
                LoadUsers();
                MessageBox.Show("Пользователь добавлен");
                addWindow.Close();
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

    public class UserView
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string RoleName { get; set; }
        public bool IsFrozen { get; set; }
    }
}
