using AutoService.AutoServiceWindowws.ClientsWindowws;
using AutoService.AutoServiceWindowws.ServicesWindowws;
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

            Refresh(0);

            clients = new List<Client>(DBConnection.AutoServiceEntities.Client.ToList());
            ClientsLV.ItemsSource = clients;
            this.DataContext = this;
        }

        private void Refresh(int i)
        {
            var allClients = DBConnection.AutoServiceEntities.Client.ToList();
            var filtered = allClients.AsQueryable();

            var searchText = SearchTB.Text.ToLower();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(x => x.FirstName.ToLower().Contains(searchText) || x.LastName.ToLower().Contains(searchText) || x.Patronymic.ToLower().Contains(searchText));
            }

            ClientsLV.ItemsSource = filtered.ToList();
            CountRecordTBL.Text = $"{filtered.Count()} из {allClients.Count}";
        }

        private void RefreshLV()
        {
            ClientsLV.ItemsSource = DBConnection.AutoServiceEntities.Client.ToList();

        }

        private void PART_ContentHost_TextChanged(object sender, TextChangedEventArgs e) //Поиск текста
        {
            Refresh(0);
        }

        private void AddClientBTN_Click(object sender, RoutedEventArgs e)
        {
            AddNewClientWindoww addNewClientWindoww = new AddNewClientWindoww();
            addNewClientWindoww.ShowDialog();
            Refresh(0);
            RefreshLV();
        }

        private void EditCkientBTN_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsLV.SelectedItem is Client client)
            {
                ClientsLV.SelectedItem = null;
                EditClientWindoww editClientWindow = new EditClientWindoww();
                editClientWindow.ShowDialog();
            }
        }

        private void RemoveClientBTN_Click(object sender, RoutedEventArgs e)
        {

        }

        private void InfoClientBTN_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Client client)
            {
                Client selectedClient = client;

                InfoClientWindoww infoClientWindow = new InfoClientWindoww(selectedClient);
                infoClientWindow.ShowDialog();

                Refresh(0);
            }
            RefreshLV();
        }
    }
}
