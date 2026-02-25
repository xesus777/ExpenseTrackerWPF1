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
            LoadAssemblies();
        }

        private void LoadAssemblies()
        {
            try
            {
                var assemblies = Core.Context.assembly_.ToList();
                var assemblyList = new List<AssemblyInfo>();

                foreach (var a in assemblies)
                {
                    var partAssemblies = Core.Context.partassembly_.Where(p => p.assemblyid == a.id).ToList();
                    var parts = new List<basepart_>();

                    decimal totalPrice = 0;
                    foreach (var p in partAssemblies)
                    {
                        if (p.basepart_ != null)
                        {
                            parts.Add(p.basepart_);
                            totalPrice += p.basepart_.price;
                        }
                    }

                    assemblyList.Add(new AssemblyInfo
                    {
                        Id = a.id,
                        Name = a.name,
                        Author = a.author,
                        PartsCount = parts.Count,
                        TotalPrice = totalPrice,
                        Parts = parts
                    });
                }

                AssembliesList.ItemsSource = assemblyList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки сборок: " + ex.Message);
            }
        }

        private void DeleteAssembly_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button == null) return;

                int assemblyId = (int)button.Tag;

                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить эту сборку?", "Подтверждение", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    var partAssemblies = Core.Context.partassembly_.Where(p => p.assemblyid == assemblyId).ToList();
                    foreach (var pa in partAssemblies)
                    {
                        Core.Context.partassembly_.Remove(pa);
                    }

                    var assembly = Core.Context.assembly_.FirstOrDefault(a => a.id == assemblyId);
                    if (assembly != null)
                    {
                        Core.Context.assembly_.Remove(assembly);
                    }

                    Core.Context.SaveChanges();

                    LoadAssemblies();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении: " + ex.Message);
            }
        }
    }

    public class AssemblyInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public int PartsCount { get; set; }
        public decimal TotalPrice { get; set; }
        public List<basepart_> Parts { get; set; }
    }
}
