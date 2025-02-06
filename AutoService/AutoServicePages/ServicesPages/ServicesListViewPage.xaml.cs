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

            Refresh(0);

            priceSlider.Value = 50000;

            services = new List<Service>(DBConnection.AutoServiceEntities.Service.ToList());
            ServicesLV.ItemsSource = services;
            this.DataContext = this;
        }

        private void RegistrServiceBTN_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Service service)
            {
                Service selectedService = service;

                InfoServiceWindoww infoServiceWindoww = new InfoServiceWindoww(selectedService);
                infoServiceWindoww.ShowDialog();

                Refresh(0);
            }
            RefreshLV();

            CleaningtheFilter();
        }

        private void Refresh(int i)
        {
            var allService = DBConnection.AutoServiceEntities.Service.ToList();
            var filtered = allService.AsQueryable();

            var searchText = SearchTB.Text.ToLower();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(x => x.Title.ToLower().Contains(searchText) ||
                                                (x.Description != null && x.Description.ToLower().Contains(searchText)));
            }

            if (LessBTN.IsEnabled == true && LargerBTN.IsEnabled == false)
                filtered = filtered.OrderBy(x => x.NewCost);
            else if (LessBTN.IsEnabled == false && LargerBTN.IsEnabled == true)
                filtered = filtered.OrderByDescending(x => x.NewCost);

            filtered = filtered.Where(x => x.NewCost <= priceSlider.Value);
            priceslvalue.Text = $"{Math.Round(priceSlider.Value, 2)} рублей";

            ServicesLV.ItemsSource = filtered.ToList();
            CountRecordTBL.Text = $"{filtered.Count()} из {allService.Count}";
        }

        private void RefreshLV()
        {
            ServicesLV.ItemsSource = DBConnection.AutoServiceEntities.Service.ToList();

        }

        private void PART_ContentHost_TextChanged(object sender, TextChangedEventArgs e) //Поиск текста
        {
            Refresh(0);
        }

        private void CleaningtheFilter()
        {
            // Сброс кнопок фильтрации
            LessBTN.Background = new SolidColorBrush(Color.FromRgb(255, 156, 26));
            LargerBTN.Background = new SolidColorBrush(Color.FromRgb(255, 156, 26));
            LessBTN.IsEnabled = true;
            LargerBTN.IsEnabled = true;

            // Очистка поля поиска
            SearchTB.Text = "";

            // Сброс слайдера
            priceSlider.Value = 50000;

            // Обновление текста для слайдера
            priceslvalue.Text = $"{Math.Round(priceSlider.Value, 2)} рублей";

            // Получение всех услуг
            var allService = DBConnection.AutoServiceEntities.Service.ToList();
            var filtered = allService.AsQueryable();

            // Устанавливаем все элементы в ListView без фильтрации
            ServicesLV.ItemsSource = filtered.ToList();
            CountRecordTBL.Text = $"{filtered.Count()} из {allService.Count}";
        }

        private void LargerBTN_Click(object sender, RoutedEventArgs e)
        {
            LargerBTN.Background = new SolidColorBrush(Color.FromRgb(0, 28, 112));
            LessBTN.Background = new SolidColorBrush(Color.FromRgb(255, 156, 26));


            LargerBTN.IsEnabled = false;
            LessBTN.IsEnabled = true;

            Refresh(0);
        }

        private void LessBTN_Click(object sender, RoutedEventArgs e)
        {
            LessBTN.Background = new SolidColorBrush(Color.FromRgb(0, 28, 112));
            LargerBTN.Background = new SolidColorBrush(Color.FromRgb(255, 156, 26));


            LessBTN.IsEnabled = false;
            LargerBTN.IsEnabled = true;

            Refresh(0);
        }

        private void ResetBTN_Click(object sender, RoutedEventArgs e)
        {
            // Сброс кнопок фильтрации
            LessBTN.Background = new SolidColorBrush(Color.FromRgb(255, 156, 26));
            LargerBTN.Background = new SolidColorBrush(Color.FromRgb(255, 156, 26));
            LessBTN.IsEnabled = true;
            LargerBTN.IsEnabled = true;

            // Очистка поля поиска
            SearchTB.Text = "";

            // Сброс слайдера
            priceSlider.Value = 50000;

            // Обновление текста для слайдера
            priceslvalue.Text = $"{Math.Round(priceSlider.Value, 2)} рублей";

            // Получение всех услуг
            var allService = DBConnection.AutoServiceEntities.Service.ToList();
            var filtered = allService.AsQueryable();

            // Устанавливаем все элементы в ListView без фильтрации
            ServicesLV.ItemsSource = filtered.ToList();
            CountRecordTBL.Text = $"{filtered.Count()} из {allService.Count}";
        }


        private void AddServiceBTN_Click(object sender, RoutedEventArgs e)
        {
            AddServiceWindoww addServiceWindoww = new AddServiceWindoww();
            addServiceWindoww.ShowDialog();
            Refresh(0);
            RefreshLV();

            CleaningtheFilter();

        }

        private void EditSeviceBTN_Click(object sender, RoutedEventArgs e)
        {
            if (ServicesLV.SelectedItem is Service service)
            {
                ServicesLV.SelectedItem = null;
                EditServiceWindoww editServiceWindoww = new EditServiceWindoww(service);
                editServiceWindoww.ShowDialog();
            }

            Refresh(0);
            RefreshLV();

            CleaningtheFilter();
        }

        private void RemoveServiceBTN_Click(object sender, RoutedEventArgs e)
        {
            if (ServicesLV.SelectedItem is Service service)
            {
                var hasClientServiceRecords = DBConnection.AutoServiceEntities.ClientService.Any(x => x.ServiceID == service.ID);

                if (hasClientServiceRecords)
                {
                    MessageBox.Show("Удаление невозможно!\nНа данную услугу существует записях.",
                        "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить услугу {service.Title}?",
                        "", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

                    if (result == MessageBoxResult.Yes)
                    {
                        DBConnection.AutoServiceEntities.Service.Remove(service);
                        DBConnection.AutoServiceEntities.SaveChanges();
                    }
                }
            }

            Refresh(0);
            RefreshLV();

            CleaningtheFilter();
        }

        private void priceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Refresh(0);
        }
    }
}
