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
using AutoService.AutoServicePages;
using AutoService.AutoServicePages.ClientsPages;
using AutoService.AutoServicePages.ServicesPages;
using AutoService.AutoServicePages.VisitRegistrationPages;

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
            var result = MessageBox.Show("Вы действительно хотите выйти?", "", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }

        }

        private void ClientsHyperlink_Click(object sender, RoutedEventArgs e)
        {
            navigation_Frame.NavigationService.Navigate(new ClientsListViewPage());
        }

        private void ServicesHyperlink_Click(object sender, RoutedEventArgs e)
        {
            navigation_Frame.NavigationService.Navigate(new ServicesListViewPage());
        }

        private void VisitRegHyperlink_Click(object sender, RoutedEventArgs e)
        {
            navigation_Frame.NavigationService.Navigate(new VisitRegViewPage());
        }
    }
}
