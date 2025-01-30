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

namespace AutoService.AutoServicePages.ServicesPages
{
    /// <summary>
    /// Логика взаимодействия для ServicesListViewPage.xaml
    /// </summary>
    public partial class ServicesListViewPage : Page
    {
        public static List<Service> services { get; set; }
        public ServicesListViewPage()
        {
            InitializeComponent();
            services = new List<Service>(DBConnection.AutoServiceEntities.Service.ToList());
            ServicesLV.ItemsSource = services;
            this.DataContext = this;
        }
    }
}
