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
    public partial class SavedAssembliesPage : Page
    {
        public SavedAssembliesPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var assemblies = Core.Context.assembly_.Include("partassembly").ToList();
            AssembliesListView.ItemsSource = assemblies;
        }

        private void AssembliesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = AssembliesListView.SelectedItem as assembly_;
            if (selected != null)
            {
                string details = $"Сборка: {selected.name}\nАвтор: {selected.author}\n\nКомплектующие:\n";
                foreach (var pa in selected.partassembly_)
                {
                    details += $"- {pa.basepart_.name}\n";
                }
                MessageBox.Show(details);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
