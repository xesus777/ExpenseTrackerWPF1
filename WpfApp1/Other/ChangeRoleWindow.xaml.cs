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
using System.Windows.Shapes;

namespace WpfApp1.Other
{
    public partial class ChangeRoleWindow : Window
    {
        private Users _user;

        public ChangeRoleWindow(Users user)
        {
            InitializeComponent();
            _user = user;
            UserLoginText.Text = $"Пользователь: {user.Login}";
            RolesListBox.ItemsSource = Core.Context.Roles.ToList();

            var currentRole = Core.Context.Roles.FirstOrDefault(r => r.RoleID == user.RoleID);
            if (currentRole != null)
                RolesListBox.SelectedItem = currentRole;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedRole = RolesListBox.SelectedItem as Roles;
            if (selectedRole != null)
            {
                _user.RoleID = selectedRole.RoleID;
                Core.Context.SaveChanges();
                MessageBox.Show($"Роль пользователя {_user.Login} изменена на {selectedRole.RoleName}");
                Close();
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
