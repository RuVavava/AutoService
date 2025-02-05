using AutoService.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    /// Логика взаимодействия для InfoClientWindoww.xaml
    /// </summary>
    public partial class InfoClientWindoww : Window
    {
        public static List<Client> clients {  get; set; }
        Client contextClient;
        public InfoClientWindoww(Client clients)
        {
            InitializeComponent();
            contextClient = clients;
            Refresh();
            this.DataContext = this;
        }

        public void Refresh()
        {
            Photo.Source = new BitmapImage(new Uri(contextClient.PhotoPath, UriKind.Relative));
            FirstNameTB.Text = contextClient.FirstName;
            LastNameTB.Text = contextClient.LastName;
            PatronymicTB.Text = contextClient.Patronymic;
            EmailTB.Text = contextClient.Email;
            PhoneTB.Text = contextClient.Phone;
            GenderTB.Text = contextClient.Gender.Name;
            DateBHTB.Text = contextClient.Birthday.ToString();
            DateRegTB.Text = contextClient.RegistrationDate.ToString();
        }

        private void OKBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
