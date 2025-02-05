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
using AutoService.DB;

namespace AutoService.AutoServiceWindowws.ClientsWindowws
{
    /// <summary>
    /// Логика взаимодействия для AddNewClientWindoww.xaml
    /// </summary>
    public partial class AddNewClientWindoww : Window
    {
        public static List<Client> clients {  get; set; }
        public static List<Gender> genders { get; set; }
        public static Client client = new Client();

        bool availabilityMainIMG = false;
        public AddNewClientWindoww()
        {
            InitializeComponent();
            clients = new List<Client>(DBConnection.AutoServiceEntities.Client.ToList());
            genders = DBConnection.AutoServiceEntities.Gender.ToList();
            this.DataContext = this;
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

        private void TextOnly(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = IsTextNotAlphabetic(e.Text);
        }

        private static bool IsTextNotAlphabetic(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^а-яА-Я]");
            return reg.IsMatch(str);
        }

        private void AddPhotoPathIMGBTN_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = $"/Клиенты/{openFileDialog.SafeFileName}";

                MainMG.Source = new BitmapImage(new Uri(selectedImagePath, UriKind.Relative));

                client.PhotoPath = selectedImagePath;

                availabilityMainIMG = true;

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
                if (string.IsNullOrWhiteSpace(FirstNameTB.Text) ||
                    string.IsNullOrWhiteSpace(LastNameTB.Text) ||
                    string.IsNullOrWhiteSpace(PatronymicTB.Text) ||
                    string.IsNullOrWhiteSpace(EmailTB.Text) ||
                    string.IsNullOrWhiteSpace(PhoneTB.Text))
                {
                    error.AppendLine("Заполните все поля!");
                    MessageBox.Show(error.ToString());
                    return;
                }

                var existingClient = DBConnection.AutoServiceEntities.Client
                    .FirstOrDefault(s => s.LastName.Equals(LastNameTB.Text, StringComparison.OrdinalIgnoreCase) ||
                                         s.FirstName.Equals(FirstNameTB.Text, StringComparison.OrdinalIgnoreCase) ||
                                         s.Patronymic.Equals(PatronymicTB.Text, StringComparison.OrdinalIgnoreCase) ||
                                         s.Phone.Equals(PhoneTB.Text, StringComparison.OrdinalIgnoreCase) ||
                                         s.Email.Equals(EmailTB.Text, StringComparison.OrdinalIgnoreCase));

                if (existingClient != null && existingClient.ID != client.ID)
                {
                    MessageBox.Show("Такой клиент уже существует.", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (availabilityMainIMG == false)
                {
                    MessageBox.Show("Добавьте главную фотографию услуги!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Проверьте верность введенных данных: \nФИО: {FirstNameTB.Text} {LastNameTB.Text} {PatronymicTB.Text}, \n" +
                                             $"Дата рождения: {DateBHCal.SelectedDate?.ToString("dd.MM.yyyy")}\n" +
                                             $"Контактные данные: {EmailTB.Text} {PhoneTB.Text}", "",
                                             MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

                if (result == MessageBoxResult.Yes)
                {
                    client.FirstName = FirstNameTB.Text.Trim();
                    client.LastName = LastNameTB.Text.Trim();
                    client.Patronymic = PatronymicTB.Text.Trim();
                    client.Email = EmailTB.Text.Trim();
                    client.RegistrationDate = DateTime.Now;
                    client.Phone = PhoneTB.Text.Trim();

                    if (DateBHCal.SelectedDate != null && (DateTime.Now - (DateTime)DateBHCal.SelectedDate).TotalDays < 365 * 18 + 4)
                    {
                        MessageBox.Show("Сотрудник не может быть младше 18 лет.");
                    }
                    else
                    {
                        client.Birthday = DateBHCal.SelectedDate;
                    }

                    client.GenderCode = (GenderCB.SelectedItem as Gender).Code;
                    DBConnection.AutoServiceEntities.Client.Add(client);
                    DBConnection.AutoServiceEntities.SaveChanges();
                    MessageBox.Show("Клиент успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
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
    }
}
