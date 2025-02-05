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

namespace AutoService.AutoServiceWindowws.ClientsWindowws
{
    /// <summary>
    /// Логика взаимодействия для EditClientWindoww.xaml
    /// </summary>
    public partial class EditClientWindoww : Window
    {
        public static List<Client> clients {  get; set; }
        public static List<Gender> genders { get; set; }
        Client contextClient;
        public EditClientWindoww(Client clients)
        {
            InitializeComponent();
            contextClient = clients;
            Refresh();
            this.DataContext = this;
        }

        public void Refresh()
        {
            genders = DBConnection.AutoServiceEntities.Gender.ToList();

            Photo.Source = new BitmapImage(new Uri(contextClient.PhotoPath, UriKind.Relative));
            FirstNameTB.Text = contextClient.FirstName;
            LastNameTB.Text = contextClient.LastName;
            PatronymicTB.Text = contextClient.Patronymic;
            EmailTB.Text = contextClient.Email;
            PhoneTB.Text = contextClient.Phone;
            DateBHTB.Text = contextClient.Birthday.ToString();
            DateRegTB.Text = contextClient.RegistrationDate.ToString();

            GenderCB.SelectedIndex = Convert.ToInt16(contextClient.GenderCode) - 1;
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


        private void EditPhotoPathIMGBTN_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = $"/Клиенты/{openFileDialog.SafeFileName}";

                Photo.Source = new BitmapImage(new Uri(selectedImagePath, UriKind.Relative));

                contextClient.PhotoPath  = selectedImagePath;
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
                Client client = contextClient;
                if (string.IsNullOrWhiteSpace(FirstNameTB.Text) || LastNameTB.Text.Trim() == "" || PatronymicTB.Text.Trim() == "" || EmailTB.Text.Trim() == "" || PhoneTB.Text.Trim() == "")
                {
                    error.AppendLine("Заполните все поля!");
                    return;
                }
                if (error.Length > 0)
                {
                    MessageBox.Show(error.ToString());
                }

                var existingClient = DBConnection.AutoServiceEntities.Client.FirstOrDefault(s => s.LastName.Equals(LastNameTB.Text, StringComparison.OrdinalIgnoreCase) ||
                s.FirstName.Equals(FirstNameTB.Text, StringComparison.OrdinalIgnoreCase) ||
                s.Patronymic.Equals(PatronymicTB.Text, StringComparison.OrdinalIgnoreCase) ||
                s.Phone.Equals(PhoneTB.Text, StringComparison.OrdinalIgnoreCase) ||
                s.Email.Equals(EmailTB.Text, StringComparison.OrdinalIgnoreCase) ||
                s.LastName.Equals(LastNameTB.Text, StringComparison.OrdinalIgnoreCase));

                if (existingClient != null && existingClient.ID != client.ID)
                {
                    MessageBox.Show("Такой клиент уже существует.", "ОШИБКА", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var result = MessageBox.Show($"Проверьте верность введенных данных: \nФИО: {FirstNameTB.Text} {LastNameTB.Text} {PatronymicTB.Text}, \n" +
                    $"Дата рождения: {DateBHTB.Text}" +
                    $"Контактные данные: {EmailTB.Text} {PhoneTB.Text}", "",
                    MessageBoxButton.YesNo, MessageBoxImage.Asterisk);


                if (result == MessageBoxResult.Yes)
                {
                    client.FirstName = FirstNameTB.Text;
                    client.LastName = LastNameTB.Text;
                    client.Patronymic = PatronymicTB.Text;
                    client.Email = EmailTB.Text;
                    client.Phone = PhoneTB.Text;
                    if (DateBHTB.SelectedDate != null && (DateTime.Now - (DateTime)DateBHTB.SelectedDate).TotalDays < 365 * 18 + 4)
                    {
                        MessageBox.Show("Клиент не может быть младше 18 лет.");
                    }
                    else
                    {
                        client.Birthday = Convert.ToDateTime(DateBHTB.Text);
                    }
                    client.GenderCode = (GenderCB.SelectedItem as Gender).Code;
                    DBConnection.AutoServiceEntities.SaveChanges();

                    FirstNameTB.Text = String.Empty;
                    LastNameTB.Text = String.Empty;
                    PatronymicTB.Text = String.Empty;
                    EmailTB.Text = String.Empty;
                    PhoneTB.Text = String.Empty;

                    DBConnection.AutoServiceEntities.SaveChanges();
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
