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

namespace AutoService.AutoServiceWindowws.ServicesWindowws
{
    /// <summary>
    /// Логика взаимодействия для AddServiceWindoww.xaml
    /// </summary>
    public partial class AddServiceWindoww : Window
    {
        public static List<Service> services { get; set; }
        public static Service service = new Service();

        bool availabilityMainIMG = false;
        public AddServiceWindoww()
        {
            InitializeComponent();
            services = new List<Service>(DBConnection.AutoServiceEntities.Service.ToList());
        }

        private void NumericOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);

        }

        private static bool IsTextNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9]");
            return reg.IsMatch(str);

        }

        private void AddMainImagePathBTN_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = $"/Услуги/{openFileDialog.SafeFileName}";

                MainMG.Source = new BitmapImage(new Uri(selectedImagePath, UriKind.Relative));

                service.MainImagePath = selectedImagePath;

                availabilityMainIMG = true;

            }
        }

        private void OKBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder error = new StringBuilder();
                if (string.IsNullOrWhiteSpace(NameServiceTB.Text) || CostServiceTB.Text.Trim() == "" || TimeServiceTB.Text.Trim() == "")

                {
                    error.AppendLine("Заполните все поля!");
                    return;
                }

                if (error.Length > 0)
                {
                    MessageBox.Show(error.ToString());
                }

                if (int.Parse(SaleServiceTB.Text) < 0)
                {
                    MessageBox.Show("Скидка не может быть меньше 0!", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (int.Parse(SaleServiceTB.Text) > 100)
                {
                    MessageBox.Show("Скидка не может быть больше 100!", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (int.Parse(TimeServiceTB.Text) < 0)
                {
                    MessageBox.Show("Время услуги не может быть отрицательным!", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (int.Parse(CostServiceTB.Text) < 0)
                {
                    MessageBox.Show("Цена не может быть ниже нуля!", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var existingService = DBConnection.AutoServiceEntities.Service.FirstOrDefault(s => s.Title.Equals(NameServiceTB.Text, StringComparison.OrdinalIgnoreCase));

                if (existingService != null && existingService.ID != service.ID)
                {
                    MessageBox.Show("Услуга с таким наименованием уже существует.", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                
                    var result = MessageBox.Show($"Проверьте верность введенных данных:\nНаименование: {NameServiceTB.Text}, \nСтоимость: {CostServiceTB.Text}, " +
                        $"Скидка:, {SaleServiceTB.Text}, \nДлительность: {TimeServiceTB.Text} минут, \nОписание: {DegrServiceTB.Text}", "",
                        MessageBoxButton.YesNo, MessageBoxImage.Asterisk);


                    if (result == MessageBoxResult.Yes)
                    {

                        service.Title = NameServiceTB.Text.Trim();
                        service.Description = DegrServiceTB.Text.Trim();
                        service.Cost = int.Parse(CostServiceTB.Text.Trim());
                        service.Discount = int.Parse(SaleServiceTB.Text.Trim());
                        service.DurationInMin = int.Parse(TimeServiceTB.Text.Trim());

                                        if (availabilityMainIMG == false)
                {
                    service.MainImagePath = "";
                }

                        DBConnection.AutoServiceEntities.Service.Add(service);
                        DBConnection.AutoServiceEntities.SaveChanges();

                        this.Close();
                    }
                


            }
            catch
            {
                MessageBoxResult result = MessageBox.Show("Произошла ошибка!\nНеобходимо перезагрузить программу!", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);

                if (result == MessageBoxResult.OK)
                {
                    //ПЕРЕЗАПУСК ПРОГРАММЫ
                    string exePath = Process.GetCurrentProcess().MainModule.FileName;
                    Process.Start(exePath);
                    Application.Current.Shutdown();
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
    }
}
