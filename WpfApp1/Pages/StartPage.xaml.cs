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
            var masters = Core.Context.Users
                .Where(u => u.RoleId == 2 && (u.IsFrozen == false || u.IsFrozen == null))
                .ToList();

            var masterServicesList = new List<MasterServiceInfo>();

            foreach (var master in masters)
            {
                var services = (from ms in Core.Context.MasterServices
                                join st in Core.Context.ServiceTypes on ms.ServiceTypeId equals st.Id
                                where ms.MasterId == master.Id
                                select new { st.Name, st.Price, st.Duration }).ToList();

                if (services.Any())
                {
                    masterServicesList.Add(new MasterServiceInfo
                    {
                        MasterId = master.Id,
                        MasterName = master.FullName,
                        ServicesList = string.Join(", ", services.Select(s => s.Name)),
                        PricesList = string.Join(", ", services.Select(s => $"{s.Price} руб")),
                        DurationList = string.Join(", ", services.Select(s => $"{s.Duration} мин"))
                    });
                }
            }

            LvMastersServices.ItemsSource = masterServicesList;
        }
    }

    public class MasterServiceInfo
    {
        public int MasterId { get; set; }
        public string MasterName { get; set; }
        public string ServicesList { get; set; }
        public string PricesList { get; set; }
        public string DurationList { get; set; }
    }
}
