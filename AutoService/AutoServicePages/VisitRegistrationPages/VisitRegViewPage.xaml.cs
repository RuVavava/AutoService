using AutoService.DB;
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

namespace AutoService.AutoServicePages.VisitRegistrationPages
{
    /// <summary>
    /// Логика взаимодействия для VisitRegViewPage.xaml
    /// </summary>
    public partial class VisitRegViewPage : Page
    {
        public static List<ClientService> clientServices {  get; set; }
        public VisitRegViewPage()
        {
            InitializeComponent();
            clientServices = new List<ClientService>(DBConnection.AutoServiceEntities.ClientService.ToList());
            NearestSCLV.ItemsSource = clientServices;
            this.DataContext = this;
        }
    }
}
