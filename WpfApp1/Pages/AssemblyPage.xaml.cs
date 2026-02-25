using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
        private Dictionary<int, string> partCharacteristics = new Dictionary<int, string>();

        private basepart_ cpu;
        private basepart_ motherboard;
        private basepart_ cooler;
        private basepart_ gpu;
        private basepart_ ram;
        private basepart_ pcCase;
        private basepart_ powerSupply;
        private basepart_ storage;

        public AssemblyPage()
        {
            InitializeComponent();

            if (TypeFilter != null && ManufacturerFilter != null)
            {
                LoadFilters();
            }

            LoadParts();
            UpdateSelectedPartsDisplay();
        }

        private void LoadFilters()
        {
            try
            {
                var types = Core.Context.parttype_.ToList();
                foreach (var t in types)
                {
                    TypeFilter.Items.Add(new ComboBoxItem { Content = t.name });
                }

                var manufacturers = Core.Context.manufacturer_.ToList();
                foreach (var m in manufacturers)
                {
                    ManufacturerFilter.Items.Add(new ComboBoxItem { Content = m.name });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки фильтров: " + ex.Message);
            }
        }

        private void LoadParts()
        {
            try
            {
                allParts = Core.Context.basepart_.ToList();

                foreach (var part in allParts)
                {
                    string chars = GetCharacteristics(part);
                    partCharacteristics[part.id] = chars;
                }

                PartsList.ItemsSource = allParts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки комплектующих: " + ex.Message);
            }
        }

        private string GetCharacteristics(basepart_ part)
        {
            try
            {
                if (part.parttype_ != null)
                {
                    if (part.parttype_.name == "CPU")
                    {
                        var cpu = Core.Context.cpu_.FirstOrDefault(c => c.id == part.id);
                        if (cpu != null)
                        {
                            var socket = Core.Context.socket_.FirstOrDefault(s => s.id == cpu.socketid);
                            return $"Сокет: {socket?.name}, Ядер: {cpu.numberofcores}, Частота: {cpu.basecorefrequency} ГГц, TDP: {cpu.thermalpower} Вт";
                        }
                    }
                    else if (part.parttype_.name == "GPU")
                    {
                        var gpu = Core.Context.gpu_.FirstOrDefault(g => g.id == part.id);
                        if (gpu != null)
                        {
                            return $"Видеопамять: {gpu.videomemory} ГБ, Частота: {gpu.chipfrequency} МГц, Рекомендуемый БП: {gpu.recommendpower} Вт";
                        }
                    }
                    else if (part.parttype_.name == "RAM")
                    {
                        var ram = Core.Context.ram_.FirstOrDefault(r => r.id == part.id);
                        if (ram != null)
                        {
                            var memType = Core.Context.memorytype_.FirstOrDefault(m => m.id == ram.memorytypeid);
                            return $"Тип: {memType?.name}, Объем: {ram.capacity} ГБ ({ram.count} планки), Частота: {ram.ghz} МГц";
                        }
                    }
                    else if (part.parttype_.name == "Motherboard")
                    {
                        var mb = Core.Context.motherboard_.FirstOrDefault(m => m.id == part.id);
                        if (mb != null)
                        {
                            var socket = Core.Context.socket_.FirstOrDefault(s => s.id == mb.socketid);
                            var form = Core.Context.formfactor_.FirstOrDefault(f => f.id == mb.formfactorid);
                            var memType = Core.Context.memorytype_.FirstOrDefault(m => m.id == mb.memorytypeid);
                            return $"Сокет: {socket?.name}, Форм-фактор: {form?.name}, Тип памяти: {memType?.name}";
                        }
                    }
                    else if (part.parttype_.name == "Case")
                    {
                        var pcCase = Core.Context.case_.FirstOrDefault(c => c.id == part.id);
                        if (pcCase != null)
                        {
                            var size = Core.Context.casesize_.FirstOrDefault(s => s.id == pcCase.sizeid);
                            var supportedFormFactors = Core.Context.boardformfactorcase_
                                .Where(b => b.caseid == pcCase.id)
                                .Select(b => b.formfactor_ != null ? b.formfactor_.name : "")
                                .ToList();
                            return $"Типоразмер: {size?.name}, Поддерживает: {string.Join(", ", supportedFormFactors)}";
                        }
                    }
                    else if (part.parttype_.name == "PowerSupply")
                    {
                        var ps = Core.Context.powersupply_.FirstOrDefault(p => p.id == part.id);
                        if (ps != null)
                        {
                            return $"Мощность: {ps.power} Вт";
                        }
                    }
                    else if (part.parttype_.name == "ProcessorCooler")
                    {
                        var cooler = Core.Context.processorcooler_.FirstOrDefault(c => c.id == part.id);
                        if (cooler != null)
                        {
                            var supportedSockets = Core.Context.socketprocessorcooler_
                                .Where(s => s.processorcoolerid == cooler.id)
                                .Select(s => s.socket_ != null ? s.socket_.name : "")
                                .ToList();
                            return $"Поддерживаемые сокеты: {string.Join(", ", supportedSockets)}";
                        }
                    }
                    else if (part.parttype_.name == "StorageDevice")
                    {
                        var storage = Core.Context.storagedevice_.FirstOrDefault(s => s.id == part.id);
                        if (storage != null)
                        {
                            var type = Core.Context.storagedevicetype_.FirstOrDefault(t => t.id == storage.storagedevicetypeid);
                            return $"Тип: {type?.name}, Объем: {storage.capacity} ГБ";
                        }
                    }
                }
            }
            catch { }

            return "";
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void TypeFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ManufacturerFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            try
            {
                if (allParts == null) return;

                var filtered = allParts.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    filtered = filtered.Where(p => p.name != null && p.name.ToLower().Contains(SearchBox.Text.ToLower()));
                }

                if (TypeFilter != null && TypeFilter.SelectedItem != null && TypeFilter.SelectedIndex > 0)
                {
                    var selectedType = TypeFilter.SelectedItem as ComboBoxItem;
                    if (selectedType != null)
                    {
                        filtered = filtered.Where(p => p.parttype_ != null && p.parttype_.name == selectedType.Content.ToString());
                    }
                }

                if (ManufacturerFilter != null && ManufacturerFilter.SelectedItem != null && ManufacturerFilter.SelectedIndex > 0)
                {
                    var selectedMan = ManufacturerFilter.SelectedItem as ComboBoxItem;
                    if (selectedMan != null)
                    {
                        filtered = filtered.Where(p => p.manufacturer_ != null && p.manufacturer_.name == selectedMan.Content.ToString());
                    }
                }

                PartsList.ItemsSource = filtered.ToList();
            }
            catch { }
        }

        private void AddPart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button == null) return;

                int partId = (int)button.Tag;

                var part = allParts.FirstOrDefault(p => p.id == partId);
                if (part == null) return;

                string compatibilityError = CheckCompatibility(part);
                if (compatibilityError != null)
                {
                    MessageBox.Show(compatibilityError, "Ошибка совместимости");
                    return;
                }

                if (part.parttype_ != null)
                {
                    switch (part.parttype_.name)
                    {
                        case "CPU": cpu = part; break;
                        case "Motherboard": motherboard = part; break;
                        case "ProcessorCooler": cooler = part; break;
                        case "GPU": gpu = part; break;
                        case "RAM": ram = part; break;
                        case "Case": pcCase = part; break;
                        case "PowerSupply": powerSupply = part; break;
                        case "StorageDevice": storage = part; break;
                    }
                }

                UpdateSelectedPartsDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private string CheckCompatibility(basepart_ newPart)
        {
            try
            {
                if (newPart.parttype_ == null) return null;

                switch (newPart.parttype_.name)
                {
                    case "CPU":
                        if (motherboard != null)
                        {
                            var newCpu = Core.Context.cpu_.FirstOrDefault(c => c.id == newPart.id);
                            var mb = Core.Context.motherboard_.FirstOrDefault(m => m.id == motherboard.id);
                            if (newCpu != null && mb != null && newCpu.socketid != mb.socketid)
                            {
                                var socket1 = Core.Context.socket_.FirstOrDefault(s => s.id == newCpu.socketid)?.name;
                                var socket2 = Core.Context.socket_.FirstOrDefault(s => s.id == mb.socketid)?.name;
                                return $"Процессор требует сокет {socket1}, а материнская плата имеет {socket2}";
                            }
                        }
                        break;

                    case "Motherboard":
                        var newMb = Core.Context.motherboard_.FirstOrDefault(m => m.id == newPart.id);
                        if (newMb == null) break;

                        if (cpu != null)
                        {
                            var existingCpu = Core.Context.cpu_.FirstOrDefault(c => c.id == cpu.id);
                            if (existingCpu != null && newMb.socketid != existingCpu.socketid)
                            {
                                var socket1 = Core.Context.socket_.FirstOrDefault(s => s.id == newMb.socketid)?.name;
                                var socket2 = Core.Context.socket_.FirstOrDefault(s => s.id == existingCpu.socketid)?.name;
                                return $"Материнская плата имеет сокет {socket1}, а процессор требует {socket2}";
                            }
                        }

                        if (ram != null)
                        {
                            var existingRam = Core.Context.ram_.FirstOrDefault(r => r.id == ram.id);
                            if (existingRam != null && newMb.memorytypeid != existingRam.memorytypeid)
                            {
                                var mem1 = Core.Context.memorytype_.FirstOrDefault(m => m.id == newMb.memorytypeid)?.name;
                                var mem2 = Core.Context.memorytype_.FirstOrDefault(m => m.id == existingRam.memorytypeid)?.name;
                                return $"Материнская плата поддерживает {mem1}, а оперативная память - {mem2}";
                            }
                        }

                        if (pcCase != null)
                        {
                            var compatible = Core.Context.boardformfactorcase_
                                .Any(b => b.caseid == pcCase.id && b.formfactorid == newMb.formfactorid);
                            if (!compatible)
                            {
                                var form = Core.Context.formfactor_.FirstOrDefault(f => f.id == newMb.formfactorid)?.name;
                                return $"Корпус не поддерживает форм-фактор материнской платы {form}";
                            }
                        }
                        break;

                    case "ProcessorCooler":
                        if (cpu != null)
                        {
                            var newCooler = Core.Context.processorcooler_.FirstOrDefault(c => c.id == newPart.id);
                            var existingCpu = Core.Context.cpu_.FirstOrDefault(c => c.id == cpu.id);
                            if (newCooler != null && existingCpu != null)
                            {
                                var compatible = Core.Context.socketprocessorcooler_
                                    .Any(s => s.processorcoolerid == newCooler.id && s.socketid == existingCpu.socketid);
                                if (!compatible)
                                {
                                    var socket = Core.Context.socket_.FirstOrDefault(s => s.id == existingCpu.socketid)?.name;
                                    return $"Кулер не поддерживает сокет процессора {socket}";
                                }
                            }
                        }
                        break;

                    case "RAM":
                        if (motherboard != null)
                        {
                            var newRam = Core.Context.ram_.FirstOrDefault(r => r.id == newPart.id);
                            var mb = Core.Context.motherboard_.FirstOrDefault(m => m.id == motherboard.id);
                            if (newRam != null && mb != null && newRam.memorytypeid != mb.memorytypeid)
                            {
                                var mem1 = Core.Context.memorytype_.FirstOrDefault(m => m.id == newRam.memorytypeid)?.name;
                                var mem2 = Core.Context.memorytype_.FirstOrDefault(m => m.id == mb.memorytypeid)?.name;
                                return $"Оперативная память типа {mem1}, а материнская плата поддерживает {mem2}";
                            }
                        }
                        break;

                    case "GPU":
                        if (powerSupply != null)
                        {
                            var newGpu = Core.Context.gpu_.FirstOrDefault(g => g.id == newPart.id);
                            var ps = Core.Context.powersupply_.FirstOrDefault(p => p.id == powerSupply.id);
                            if (newGpu != null && ps != null && newGpu.recommendpower > ps.power)
                            {
                                return $"Видеокарта требует блок питания {newGpu.recommendpower} Вт, а установлен {ps.power} Вт";
                            }
                        }
                        break;

                    case "Case":
                        if (motherboard != null)
                        {
                            var newCase = Core.Context.case_.FirstOrDefault(c => c.id == newPart.id);
                            var mb = Core.Context.motherboard_.FirstOrDefault(m => m.id == motherboard.id);
                            if (newCase != null && mb != null)
                            {
                                var compatible = Core.Context.boardformfactorcase_
                                    .Any(b => b.caseid == newCase.id && b.formfactorid == mb.formfactorid);
                                if (!compatible)
                                {
                                    var form = Core.Context.formfactor_.FirstOrDefault(f => f.id == mb.formfactorid)?.name;
                                    return $"Корпус не поддерживает форм-фактор материнской платы {form}";
                                }
                            }
                        }
                        break;

                    case "PowerSupply":
                        if (gpu != null)
                        {
                            var newPs = Core.Context.powersupply_.FirstOrDefault(p => p.id == newPart.id);
                            var existingGpu = Core.Context.gpu_.FirstOrDefault(g => g.id == gpu.id);
                            if (newPs != null && existingGpu != null && newPs.power < existingGpu.recommendpower)
                            {
                                return $"Блок питания {newPs.power} Вт недостаточен для видеокарты, требуется {existingGpu.recommendpower} Вт";
                            }
                        }
                        break;
                }
            }
            catch { }

            return null;
        }

        private void UpdateSelectedPartsDisplay()
        {
            try
            {
                SelectedPartsPanel.Children.Clear();
                decimal totalPrice = 0;

                AddPartToPanel(cpu, "Процессор", ref totalPrice);
                AddPartToPanel(motherboard, "Материнская плата", ref totalPrice);
                AddPartToPanel(cooler, "Кулер", ref totalPrice);
                AddPartToPanel(gpu, "Видеокарта", ref totalPrice);
                AddPartToPanel(ram, "Оперативная память", ref totalPrice);
                AddPartToPanel(pcCase, "Корпус", ref totalPrice);
                AddPartToPanel(powerSupply, "Блок питания", ref totalPrice);
                AddPartToPanel(storage, "Накопитель", ref totalPrice);

                TotalPriceText.Text = $"ИТОГО: {totalPrice:C}";
            }
            catch { }
        }

        private void AddPartToPanel(basepart_ part, string title, ref decimal totalPrice)
        {
            try
            {
                if (part != null)
                {
                    totalPrice += part.price;

                    Border border = new Border
                    {
                        BorderBrush = new SolidColorBrush(Colors.DarkGray),
                        BorderThickness = new Thickness(0, 0, 0, 1),
                        Margin = new Thickness(0, 5, 0, 5),
                        Padding = new Thickness(5)
                    };

                    StackPanel stack = new StackPanel();

                    stack.Children.Add(new TextBlock
                    {
                        Text = title,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Colors.DarkBlue)
                    });

                    stack.Children.Add(new TextBlock
                    {
                        Text = part.name,
                        FontWeight = FontWeights.Bold
                    });

                    if (partCharacteristics.ContainsKey(part.id))
                    {
                        stack.Children.Add(new TextBlock
                        {
                            Text = partCharacteristics[part.id],
                            TextWrapping = TextWrapping.Wrap,
                            FontSize = 11,
                            Margin = new Thickness(0, 2, 0, 2)
                        });
                    }

                    stack.Children.Add(new TextBlock
                    {
                        Text = $"{part.price:C}",
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Right
                    });

                    border.Child = stack;
                    SelectedPartsPanel.Children.Add(border);
                }
                else
                {
                    Border border = new Border
                    {
                        BorderBrush = new SolidColorBrush(Colors.LightGray),
                        BorderThickness = new Thickness(0, 0, 0, 1),
                        Margin = new Thickness(0, 5, 0, 5),
                        Padding = new Thickness(5)
                    };

                    TextBlock text = new TextBlock
                    {
                        Text = $"{title}: не выбран",
                        FontStyle = FontStyles.Italic,
                        Foreground = new SolidColorBrush(Colors.Gray)
                    };

                    border.Child = text;
                    SelectedPartsPanel.Children.Add(border);
                }
            }
            catch { }
        }

        private void ClearAssembly_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите очистить всю сборку?", "Подтверждение", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                cpu = null;
                motherboard = null;
                cooler = null;
                gpu = null;
                ram = null;
                pcCase = null;
                powerSupply = null;
                storage = null;

                UpdateSelectedPartsDisplay();
            }
        }

        private void SaveAssembly_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AssemblyName.Text) || string.IsNullOrWhiteSpace(AuthorName.Text))
                {
                    MessageBox.Show("Введите название сборки и имя автора!");
                    return;
                }

                List<basepart_> selectedPartsList = new List<basepart_>();
                if (cpu != null) selectedPartsList.Add(cpu);
                if (motherboard != null) selectedPartsList.Add(motherboard);
                if (cooler != null) selectedPartsList.Add(cooler);
                if (gpu != null) selectedPartsList.Add(gpu);
                if (ram != null) selectedPartsList.Add(ram);
                if (pcCase != null) selectedPartsList.Add(pcCase);
                if (powerSupply != null) selectedPartsList.Add(powerSupply);
                if (storage != null) selectedPartsList.Add(storage);

                if (selectedPartsList.Count == 0)
                {
                    MessageBox.Show("Добавьте комплектующие в сборку!");
                    return;
                }

                var assembly = new assembly_
                {
                    name = AssemblyName.Text,
                    author = AuthorName.Text
                };

                Core.Context.assembly_.Add(assembly);
                Core.Context.SaveChanges();

                foreach (var part in selectedPartsList)
                {
                    var partAssembly = new partassembly_
                    {
                        partid = part.id,
                        assemblyid = assembly.id
                    };
                    Core.Context.partassembly_.Add(partAssembly);
                }

                Core.Context.SaveChanges();
                MessageBox.Show($"Сборка \"{AssemblyName.Text}\" успешно сохранена!");

                ClearAssembly_Click(null, null);
                AssemblyName.Clear();
                AuthorName.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }
    }
}
