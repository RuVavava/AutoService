using AutoService.AutoServiceWindowws.ServicesWindowws;
using AutoService.AutoServiceWindowws.VisitRegistrationWindowws;
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

            Refresh(0);
            clientServices = new List<ClientService>(DBConnection.AutoServiceEntities.ClientService.OrderByDescending(x => x.StartTime).ToList());
            NearestSCLV.ItemsSource = clientServices;
            this.DataContext = this;
        }

        public void Refresh(int i)
        {
            var allVisit = DBConnection.AutoServiceEntities.ClientService.ToList();
            var filtered = allVisit.AsQueryable();

            var searchText = SearchTB.Text.ToLower();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(x => x.Service.Title.ToLower().Contains(searchText) ||
                                                x.Client.FirstName.ToLower().Contains(searchText) ||
                                                x.Client.LastName.ToLower().Contains(searchText) ||
                                                x.Client.Patronymic.ToLower().Contains(searchText) ||
                                                x.Client.Email.ToLower().Contains(searchText) ||
                                                x.Client.Phone.ToLower().Contains(searchText)
                                                );
            }

            if (LessBTN.IsEnabled == true && LargerBTN.IsEnabled == false)
                filtered = filtered.OrderBy(x => x.StartTime);
            else if (LessBTN.IsEnabled == false && LargerBTN.IsEnabled == true)
                filtered = filtered.OrderByDescending(x => x.StartTime);

            var selectedDates = EventCalendar.SelectedDates;
            if (selectedDates.Count > 0)
            {
                var startDate = selectedDates.Min();
                var endDate = selectedDates.Max();
                filtered = filtered.Where(x => x.StartTime.Date >= startDate.Date && x.StartTime.Date <= endDate.Date);
            }

            NearestSCLV.ItemsSource = filtered.ToList();
            CountRecordTBL.Text = $"{filtered.Count()} из {allVisit.Count}";
        }

        public void RefreshLV()
        {
            clientServices = new List<ClientService>(DBConnection.AutoServiceEntities.ClientService.OrderByDescending(x => x.StartTime).ToList());
            NearestSCLV.ItemsSource = clientServices;
        }

        public void CleaningtheFilter()
        {
            // Сброс кнопок фильтрации
            LessBTN.Background = new SolidColorBrush(Color.FromRgb(255, 156, 26));
            LargerBTN.Background = new SolidColorBrush(Color.FromRgb(255, 156, 26));
            LessBTN.IsEnabled = true;
            LargerBTN.IsEnabled = true;

            // Очистка поля поиска
            SearchTB.Text = "";

            // Очистка выбранных дат в календаре
            EventCalendar.SelectedDates.Clear();

            // Получение всех записей (вместо ограничений только на услуги)
            var allVisit = DBConnection.AutoServiceEntities.ClientService.ToList();
            var filtered = allVisit.AsQueryable();

            // Убираем фильтрацию по поиску (по умолчанию все элементы видны)
            var searchText = SearchTB.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(x => x.Service.Title.ToLower().Contains(searchText) ||
                                                x.Client.FirstName.ToLower().Contains(searchText) ||
                                                x.Client.LastName.ToLower().Contains(searchText) ||
                                                x.Client.Patronymic.ToLower().Contains(searchText) ||
                                                x.Client.Email.ToLower().Contains(searchText) ||
                                                x.Client.Phone.ToLower().Contains(searchText)
                                               );
            }

            // Сброс сортировки
            filtered = filtered.OrderByDescending(x => x.StartTime);

            // Устанавливаем все элементы в ListView без фильтрации
            NearestSCLV.ItemsSource = filtered.ToList();
            CountRecordTBL.Text = $"{filtered.Count()} из {allVisit.Count}";
        }


        private void PART_ContentHost_TextChanged(object sender, TextChangedEventArgs e) //Поиск текста
        {
            Refresh(0);
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
            CleaningtheFilter();
        }

        private void AddVistBTN_Click(object sender, RoutedEventArgs e)
        {
            AddVisitRegWindoww addVisitRegWindoww = new AddVisitRegWindoww();
            addVisitRegWindoww.ShowDialog();
            Refresh(0);
            RefreshLV();

            CleaningtheFilter();
        }

        private void EditVisitBTN_Click(object sender, RoutedEventArgs e)
        {
            if (NearestSCLV.SelectedItem is ClientService clientService)
            {
                // Проверяем, прошла ли услуга по дате
                var serviceDate = clientService.StartTime; // Используем StartTime из clientService

                if (serviceDate < DateTime.Now)
                {
                    MessageBox.Show("Удаление невозможно!\nУслуга уже прошла.", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить запись?",
                    "", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

                if (result == MessageBoxResult.Yes)
                {
                    DBConnection.AutoServiceEntities.ClientService.Remove(clientService);
                    DBConnection.AutoServiceEntities.SaveChanges();
                }

            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для редактирования.", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
                    Refresh(0);
                    RefreshLV();
            CleaningtheFilter();
        }


        private void EventCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh(0);
        }
    }
}
