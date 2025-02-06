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
using System.Windows.Shapes;

namespace AutoService.AutoServiceWindowws.ServicesWindowws
{
    /// <summary>
    /// Логика взаимодействия для InfoServiceWindoww.xaml
    /// </summary>
    public partial class InfoServiceWindoww : Window
    {
        public static List<Service> services { get; set; }
        Service contextService;
        public InfoServiceWindoww(Service service)
        {
            InitializeComponent();
            contextService = service;
            Refresh();
            this.DataContext = this;
        }
        public void Refresh()
        {
            MainMG.Source = new BitmapImage(new Uri(contextService.MainImagePath, UriKind.Relative));
            NameServiceTB.Text = contextService.Title;
            CostServiceTB.Text = contextService.Cost.ToString();
            SaleServiceTB.Text = contextService.Discount.ToString();
            TimeServiceTB.Text = contextService.DurationInMin.ToString();
            DegrServiceTB.Text = contextService.Description;

        }

        private void OKBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
