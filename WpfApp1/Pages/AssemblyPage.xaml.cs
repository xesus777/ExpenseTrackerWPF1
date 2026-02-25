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
    public partial class AssemblyPage : Page
    {
        private List<basepart_> allParts;
        private List<basepart_> selectedParts = new List<basepart_>();

        public AssemblyPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadManufacturers();
            LoadParts();
        }

        private void LoadManufacturers()
        {
            var manufacturers = Core.Context.manufacturer_.ToList();
            manufacturers.Insert(0, new manufacturer_ { id = 0, name = "Все производители" });
            ManufacturerFilterCombo.ItemsSource = manufacturers;
            ManufacturerFilterCombo.DisplayMemberPath = "name";
            ManufacturerFilterCombo.SelectedValuePath = "id";
            ManufacturerFilterCombo.SelectedIndex = 0;
        }

        private void LoadParts()
        {
            allParts = Core.Context.basepart_.Include("manufacturer").ToList();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = allParts.AsEnumerable();

            string searchText = SearchBox.Text.ToLower();
            if (searchText != "поиск..." && !string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(p => p.name.ToLower().Contains(searchText));
            }

            if (TypeFilterCombo.SelectedItem != null)
            {
                string selectedType = (TypeFilterCombo.SelectedItem as ComboBoxItem).Content.ToString();
                if (selectedType != "Все типы")
                {
                    filtered = filtered.Where(p => p.parttype_.name == selectedType);
                }
            }

            if (ManufacturerFilterCombo.SelectedValue != null && (int)ManufacturerFilterCombo.SelectedValue != 0)
            {
                int manId = (int)ManufacturerFilterCombo.SelectedValue;
                filtered = filtered.Where(p => p.manufacturerid == manId);
            }

            PartsListView.ItemsSource = filtered.ToList();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchBox.Text == "Поиск...") return;
            ApplyFilters();
        }

        private void TypeFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ManufacturerFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void PartsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedPart = PartsListView.SelectedItem as basepart_;
            if (selectedPart != null)
            {
                if (CheckCompatibility(selectedPart))
                {
                    selectedParts.Add(selectedPart);
                    UpdateSelectedParts();
                }
                else
                {
                    MessageBox.Show("Данное комплектующее несовместимо с текущей сборкой!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private bool CheckCompatibility(basepart_ newPart)
        {
            if (selectedParts.Count == 0) return true;

            if (newPart.parttype_.name == "CPU")
            {
                var motherboard = selectedParts.FirstOrDefault(p => p.parttype_.name == "Motherboard");
                if (motherboard != null)
                {
                    int newSocket = Core.Context.cpu_.First(c => c.id == newPart.id).socketid;
                    int boardSocket = Core.Context.motherboard_.First(m => m.id == motherboard.id).socketid;
                    if (newSocket != boardSocket) return false;
                }

                var cooler = selectedParts.FirstOrDefault(p => p.parttype_.name == "ProcessorCooler");
                if (cooler != null)
                {
                    int newSocket = Core.Context.cpu_.First(c => c.id == newPart.id).socketid;
                    bool coolerSupports = Core.Context.socketprocessorcooler_.Any(sc => sc.processorcoolerid == cooler.id && sc.socketid == newSocket);
                    if (!coolerSupports) return false;
                }
            }

            if (newPart.parttype_.name == "Motherboard")
            {
                var cpu = selectedParts.FirstOrDefault(p => p.parttype_.name == "CPU");
                if (cpu != null)
                {
                    int cpuSocket = Core.Context.cpu_.First(c => c.id == cpu.id).socketid;
                    int newBoardSocket = Core.Context.motherboard_.First(m => m.id == newPart.id).socketid;
                    if (cpuSocket != newBoardSocket) return false;
                }

                var casePart = selectedParts.FirstOrDefault(p => p.parttype_.name == "Case");
                if (casePart != null)
                {
                    int boardFormFactor = Core.Context.motherboard_.First(m => m.id == newPart.id).formfactorid;
                    bool caseSupports = Core.Context.boardformfactorcase_.Any(bf => bf.caseid == casePart.id && bf.formfactorid == boardFormFactor);
                    if (!caseSupports) return false;
                }

                var ram = selectedParts.FirstOrDefault(p => p.parttype_.name == "RAM");
                if (ram != null)
                {
                    int newBoardMemType = Core.Context.motherboard_.First(m => m.id == newPart.id).memorytypeid;
                    int ramMemType = Core.Context.ram_.First(r => r.id == ram.id).memorytypeid;
                    if (newBoardMemType != ramMemType) return false;
                }
            }

            if (newPart.parttype_.name == "RAM")
            {
                var motherboard = selectedParts.FirstOrDefault(p => p.parttype_.name == "Motherboard");
                if (motherboard != null)
                {
                    int newRamMemType = Core.Context.ram_.First(r => r.id == newPart.id).memorytypeid;
                    int boardMemType = Core.Context.motherboard_.First(m => m.id == motherboard.id).memorytypeid;
                    if (newRamMemType != boardMemType) return false;
                }
            }

            if (newPart.parttype_.name == "Case")
            {
                var motherboard = selectedParts.FirstOrDefault(p => p.parttype_.name == "Motherboard");
                if (motherboard != null)
                {
                    int boardFormFactor = Core.Context.motherboard_.First(m => m.id == motherboard.id).formfactorid;
                    bool caseSupports = Core.Context.boardformfactorcase_.Any(bf => bf.caseid == newPart.id && bf.formfactorid == boardFormFactor);
                    if (!caseSupports) return false;
                }
            }

            if (newPart.parttype_.name == "ProcessorCooler")
            {
                var cpu = selectedParts.FirstOrDefault(p => p.parttype_.name == "CPU");
                if (cpu != null)
                {
                    int cpuSocket = Core.Context.cpu_.First(c => c.id == cpu.id).socketid;
                    bool coolerSupports = Core.Context.socketprocessorcooler_.Any(sc => sc.processorcoolerid == newPart.id && sc.socketid == cpuSocket);
                    if (!coolerSupports) return false;
                }
            }

            if (newPart.parttype_.name == "GPU")
            {
                var psu = selectedParts.FirstOrDefault(p => p.parttype_.name == "PowerSupply");
                if (psu != null)
                {
                    int gpuPower = Core.Context.gpu_.First(g => g.id == newPart.id).recommendpower ?? 0;
                    int psuPower = Core.Context.powersupply_.First(ps => ps.id == psu.id).power;
                    if (gpuPower > psuPower) return false;
                }
            }

            if (newPart.parttype_.name == "PowerSupply")
            {
                var gpu = selectedParts.FirstOrDefault(p => p.parttype_.name == "GPU");
                if (gpu != null)
                {
                    int gpuPower = Core.Context.gpu_.First(g => g.id == gpu.id).recommendpower ?? 0;
                    int newPsuPower = Core.Context.powersupply_.First(ps => ps.id == newPart.id).power;
                    if (gpuPower > newPsuPower) return false;
                }
            }

            return true;
        }

        private void UpdateSelectedParts()
        {
            SelectedPartsList.ItemsSource = null;
            SelectedPartsList.ItemsSource = selectedParts;

            decimal total = selectedParts.Sum(p => p.price);
            TotalPriceText.Text = $"Общая цена: {total} руб.";

            CheckAllCompatibility();
        }

        private void CheckAllCompatibility()
        {
            string errors = "";

            if (selectedParts.Count > 0)
            {
                var cpu = selectedParts.FirstOrDefault(p => p.parttype_.name == "CPU");
                var motherboard = selectedParts.FirstOrDefault(p => p.parttype_.name == "Motherboard");
                var cooler = selectedParts.FirstOrDefault(p => p.parttype_.name == "ProcessorCooler");
                var ram = selectedParts.FirstOrDefault(p => p.parttype_.name == "RAM");
                var casePart = selectedParts.FirstOrDefault(p => p.parttype_.name == "Case");
                var gpu = selectedParts.FirstOrDefault(p => p.parttype_.name == "GPU");
                var psu = selectedParts.FirstOrDefault(p => p.parttype_.name == "PowerSupply");

                if (cpu != null && motherboard != null)
                {
                    int cpuSocket = Core.Context.cpu_.First(c => c.id == cpu.id).socketid;
                    int boardSocket = Core.Context.motherboard_.First(m => m.id == motherboard.id).socketid;
                    if (cpuSocket != boardSocket)
                        errors += "- Несовместимость сокета CPU и материнской платы\n";
                }

                if (cpu != null && cooler != null)
                {
                    int cpuSocket = Core.Context.cpu_.First(c => c.id == cpu.id).socketid;
                    bool coolerSupports = Core.Context.socketprocessorcooler_.Any(sc => sc.processorcoolerid == cooler.id && sc.socketid == cpuSocket);
                    if (!coolerSupports)
                        errors += "- Кулер не поддерживает сокет CPU\n";
                }

                if (motherboard != null && casePart != null)
                {
                    int boardFormFactor = Core.Context.motherboard_.First(m => m.id == motherboard.id).formfactorid;
                    bool caseSupports = Core.Context.boardformfactorcase_.Any(bf => bf.caseid == casePart.id && bf.formfactorid == boardFormFactor);
                    if (!caseSupports)
                        errors += "- Форм-фактор материнской платы не поддерживается корпусом\n";
                }

                if (motherboard != null && ram != null)
                {
                    int boardMemType = Core.Context.motherboard_.First(m => m.id == motherboard.id).memorytypeid;
                    int ramMemType = Core.Context.ram_.First(r => r.id == ram.id).memorytypeid;
                    if (boardMemType != ramMemType)
                        errors += "- Тип памяти RAM не совместим с материнской платой\n";
                }

                if (gpu != null && psu != null)
                {
                    int gpuPower = Core.Context.gpu_.First(g => g.id == gpu.id).recommendpower ?? 0;
                    int psuPower = Core.Context.powersupply_.First(ps => ps.id == psu.id).power;
                    if (gpuPower > psuPower)
                        errors += "- Мощности блока питания недостаточно для видеокарты\n";
                }
            }

            CompatibilityText.Text = errors;
        }

        private void SaveAssembly_Click(object sender, RoutedEventArgs e)
        {
            if (selectedParts.Count == 0)
            {
                MessageBox.Show("Добавьте комплектующие в сборку");
                return;
            }

            if (string.IsNullOrWhiteSpace(AssemblyNameBox.Text) || AssemblyNameBox.Text == "Название сборки")
            {
                MessageBox.Show("Введите название сборки");
                return;
            }

            if (string.IsNullOrWhiteSpace(AuthorNameBox.Text) || AuthorNameBox.Text == "Автор")
            {
                MessageBox.Show("Введите имя автора");
                return;
            }

            var assembly = new assembly_
            {
                name = AssemblyNameBox.Text,
                author = AuthorNameBox.Text
            };

            Core.Context.assembly_.Add(assembly);
            Core.Context.SaveChanges();

            foreach (var part in selectedParts)
            {
                var pa = new partassembly_
                {
                    partid = part.id,
                    assemblyid = assembly.id
                };
                Core.Context.partassembly_.Add(pa);
            }

            Core.Context.SaveChanges();

            MessageBox.Show("Сборка сохранена!");

            AssemblyNameBox.Text = "Название сборки";
            AuthorNameBox.Text = "Автор";
        }

        private void ViewSavedAssemblies_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SavedAssembliesPage());
        }
    }
}
