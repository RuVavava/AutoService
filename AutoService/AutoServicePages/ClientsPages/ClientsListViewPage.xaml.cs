using AutoService.AutoServiceWindowws.ClientsWindowws;
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

namespace AutoService.AutoServicePages.ClientsPages
{
    /// <summary>
    /// Логика взаимодействия для ClientsListViewPage.xaml
    /// </summary>
    public partial class ClientsListViewPage : Page
    {
        public static List<Client> clients { get; set; }
        public ClientsListViewPage()
        {
            InitializeComponent();
            clients = new List<Client>(DBConnection.AutoServiceEntities.Client.ToList());
            ClientsLV.ItemsSource = clients;
            this.DataContext = this;
        }

        private void AddClientBTN_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
