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
using AutoService.AutoServicePages.ClientsPages;

namespace AutoService
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            navigation_Frame.NavigationService.Navigate(new ClientsListViewPage());
        }

        private void outBTN_Click(object sender, RoutedEventArgs e) //Выход из программы
        {
            Application.Current.Shutdown();
        }

        private void ClientsHyperlink_Click(object sender, RoutedEventArgs e)
        {
            navigation_Frame.NavigationService.Navigate(new ClientsListViewPage());
        }

        private void ServicesHyperlink_Click(object sender, RoutedEventArgs e)
        {
            //navigation_Frame.NavigationService.Navigate(new ClientsListViewPage());
        }

        private void VisitRegHyperlink_Click(object sender, RoutedEventArgs e)
        {
            //navigation_Frame.NavigationService.Navigate(new ClientsListViewPage());
        }
    }
}
