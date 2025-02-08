using AutoService.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace AutoService.AutoServiceWindowws.VisitRegistrationWindowws
{
    /// <summary>
    /// Логика взаимодействия для AddVisitRegWindoww.xaml
    /// </summary>
    public partial class AddVisitRegWindoww : Window
    {
        public static List<Client> clients { get; set; }
        public static List<Service> services { get; set; }
        public static List<ClientService> clientServices { get; set; }

        public static ClientService clientService = new ClientService();
        public AddVisitRegWindoww()
        {
            InitializeComponent();
            clients = new List<Client>(DBConnection.AutoServiceEntities.Client.ToList());
            services = DBConnection.AutoServiceEntities.Service.ToList();
            clientServices = DBConnection.AutoServiceEntities.ClientService.ToList();
            this.DataContext = this;
        }

        private void SearchClientTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            var allClients = DBConnection.AutoServiceEntities.Client.ToList();
            var filtered = allClients.AsQueryable();

            var searchText = SearchClientTB.Text.ToLower();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(x => x.FirstName.ToLower().Contains(searchText) || x.LastName.ToLower().Contains(searchText) || x.Patronymic.ToLower().Contains(searchText));
            }

            ClientsCB.ItemsSource = filtered;
        }

        private void SearchServiceTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            var allService = DBConnection.AutoServiceEntities.Service.ToList();
            var filtered = allService.AsQueryable();

            var searchText = SearchServiceTB.Text.ToLower();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(x => x.Title.ToLower().Contains(searchText) ||
                                                (x.Description != null && x.Description.ToLower().Contains(searchText)));
            }

            ServiceCB.ItemsSource = filtered;
        }

        private void TimeTbx_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1) && e.Text != ":")
            {
                e.Handled = true;
            }
        }

        private void TimeTbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TimeClientServiceTB.Text.Length == 5 && TimeClientServiceTB.Text[2] == ':')
            {
                int hour, minute;
                if (int.TryParse(TimeClientServiceTB.Text.Substring(0, 2), out hour) && int.TryParse(TimeClientServiceTB.Text.Substring(3, 2), out minute))
                {
                    if (hour < 0 || hour > 23 || minute < 0 || minute > 59)
                    {
                        MessageBox.Show("Введите корректное время в формате Час:Минута!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        TimeClientServiceTB.Text = string.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("Введите корректное время в формате Час:Минута!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    TimeClientServiceTB.Text = string.Empty;
                }
            }
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти?", "", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void OKBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder error = new StringBuilder();

                if (ClientsCB.SelectedIndex == -1 || DateClientServiceDP.SelectedDate == null || TimeClientServiceTB.Text.Trim() == "" ||
                    string.IsNullOrWhiteSpace(ClientsCB.Text) || string.IsNullOrWhiteSpace(ServiceCB.Text))
                {
                    error.AppendLine("Заполните все поля!");
                }

                int hour = -1;
                int minute = -1;

                if (TimeClientServiceTB.Text.Length == 5 && TimeClientServiceTB.Text[2] == ':' &&
                    int.TryParse(TimeClientServiceTB.Text.Substring(0, 2), out hour) &&
                    int.TryParse(TimeClientServiceTB.Text.Substring(3, 2), out minute))
                {
                    if (hour < 0 || hour > 23 || minute < 0 || minute > 59)
                    {
                        error.AppendLine("Введите корректное время в формате Час:Минута!");
                    }
                }
                else
                {
                    error.AppendLine("Введите корректное время в формате Час:Минута!");
                }

                if (error.Length == 0)
                {
                    DateTime selectedDate = (DateTime)DateClientServiceDP.SelectedDate;
                    DateTime selectedTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, hour, minute, 0);

                    // Проверка, не выбрано ли прошедшее время
                    if (selectedTime < DateTime.Now)
                    {
                        error.AppendLine("Вы не можете выбрать прошедшее время!");
                    }
                }

                if (error.Length > 0)
                {
                    MessageBox.Show(error.ToString());
                }
                else
                {
                    DateTime selectedDate = (DateTime)DateClientServiceDP.SelectedDate;
                    DateTime startTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, hour, minute, 0);
                    var selectedClient = ClientsCB.SelectedItem as Client;
                    var selectedService = ServiceCB.SelectedItem as Service;

                    var result = MessageBox.Show($"Проверьте верность введенных данных:\nНаименование услуги: {selectedService.Title}" +
                        $"Дата: {selectedDate.Day}.{selectedDate.Month}.{selectedDate.Year}, Время: {hour}:{minute}, " +
                        $"\nКлиент: {selectedClient.LastName} {selectedClient.FirstName} {selectedClient.Patronymic}", "", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

                    if (result == MessageBoxResult.Yes)
                    {
                        ClientService clientService = new ClientService();

                        clientService.ClientID = selectedClient.ID;
                        clientService.ServiceID = selectedService.ID;
                        clientService.StartTime = startTime;
                        clientService.Comment = CommentTB.Text;

                        DBConnection.AutoServiceEntities.ClientService.Add(clientService);
                        DBConnection.AutoServiceEntities.SaveChanges();

                        this.Close();
                    }
                }
            }
            catch
            {
                MessageBoxResult result = MessageBox.Show("Произошла ошибка!\nНеобходимо перезагрузить программу!", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);

                if (result == MessageBoxResult.OK)
                {
                    // ПЕРЕЗАПУСК ПРОГРАММЫ
                    string exePath = Process.GetCurrentProcess().MainModule.FileName;
                    Process.Start(exePath);
                    Application.Current.Shutdown();
                }
            }
        }

    }
}
