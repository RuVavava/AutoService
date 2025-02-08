using AutoService.AutoServiceWindowws.ServicesWindowws;
using AutoService.AutoServiceWindowws.VisitRegistrationWindowws;
using AutoService.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Drawing;
using System.IO;
using Xceed.Document.NET;
using Xceed.Words.NET;
using OfficeOpenXml;


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

        private void PDFBTN_Click(object sender, RoutedEventArgs e)
        {
            // Создаем новый PDF документ
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Учет посещений";

            // Добавляем страницу в документ
            PdfPage page = document.AddPage();
            page.Width = 595; // A4 ширина
            page.Height = 842; // A4 высота

            // Создаем объект XGraphics для рисования на странице
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Определяем отступы и шрифт
            double yPosition = 20;
            var font = new XFont("Arial", 12);

            // Рисуем заголовок
            gfx.DrawString("Учет посещений", font, XBrushes.Black, new XPoint(20, yPosition));
            yPosition += 20;

            var clientServices = DBConnection.AutoServiceEntities.ClientService.ToList();

            // Рисуем каждый элемент ListView
            foreach (var clientservice in clientServices)
            {

                // Строка с данными о сервисе
                string serviceInfo = $"{clientservice.ID} | {clientservice.Client.FirstName} {clientservice.Client.LastName} {clientservice.Client.Patronymic} |" +
                    $" {clientservice.Service.Title} | {clientservice.StartTime}";
                gfx.DrawString(serviceInfo, font, XBrushes.Black, new XPoint(20, yPosition));
                yPosition += 20;

                // Если список слишком длинный для одной страницы, добавляем новую страницу
                if (yPosition > page.Height - 50)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);  // Создаем новый XGraphics для новой страницы
                    yPosition = 20;
                    gfx.DrawString("Учет посещений", font, XBrushes.Black, new XPoint(20, yPosition));
                    yPosition += 20;
                }

            }

            // Сохранение PDF файла
            string filename = "ClientServicesList.pdf";
            document.Save(filename);
            Process.Start(filename); // Открываем PDF после сохранения

        }

        private void WORDBTN_Click(object sender, RoutedEventArgs e)
        {
            // Создаем новый документ Word
            var doc = DocX.Create("ClientServicesList.docx");

            // Добавляем заголовок в документ
            doc.InsertParagraph("Список услуг").FontSize(16).Bold().Alignment = Alignment.center;
            doc.InsertParagraph(); // Пустая строка для отступа

            // Получаем данные из базы данных
            var clientServices = DBConnection.AutoServiceEntities.ClientService.ToList();

            // Добавляем таблицу для услуг (заголовок таблицы)
            var table = doc.AddTable(clientServices.Count + 1, 6); // Количество строк: количество услуг + 1 для заголовков
            table.Alignment = Alignment.center;

            // Заголовки столбцов
            table.Rows[0].Cells[0].Paragraphs[0].Append("ID");
            table.Rows[0].Cells[1].Paragraphs[0].Append("Фамилия клиента");
            table.Rows[0].Cells[2].Paragraphs[0].Append("Имя клиента");
            table.Rows[0].Cells[3].Paragraphs[0].Append("Отчество клиента");
            table.Rows[0].Cells[4].Paragraphs[0].Append("Услуга");
            table.Rows[0].Cells[5].Paragraphs[0].Append("Время посещения");

            // Заполняем таблицу данными из базы данных
            for (int i = 0; i < clientServices.Count; i++)
            {
                var clientService = clientServices[i];
                table.Rows[i + 1].Cells[0].Paragraphs[0].Append(clientService.ID.ToString());
                table.Rows[i + 1].Cells[1].Paragraphs[0].Append(clientService.Client.FirstName);
                table.Rows[i + 1].Cells[2].Paragraphs[0].Append(clientService.Client.LastName);
                table.Rows[i + 1].Cells[3].Paragraphs[0].Append(clientService.Client.Patronymic);
                table.Rows[i + 1].Cells[4].Paragraphs[0].Append(clientService.Service.Title);
                table.Rows[i + 1].Cells[5].Paragraphs[0].Append(clientService.StartTime.ToString());
            }

            // Вставляем таблицу в документ
            doc.InsertTable(table);

            // Сохраняем документ
            doc.Save();

            // Открываем файл Word
            Process.Start("ClientServicesList.docx");

        }

        private void EXCELBTN_Click(object sender, RoutedEventArgs e)
        {
            // Устанавливаем лицензию для EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Получаем данные из базы данных
            var clientServices = DBConnection.AutoServiceEntities.ClientService.ToList();

            // Создаем новый пакет Excel (работа с файлом .xlsx)
            using (var package = new ExcelPackage())
            {
                // Добавляем новый рабочий лист
                var worksheet = package.Workbook.Worksheets.Add("Учет посещений");

                // Добавляем заголовки столбцов
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Фамилия клиента";
                worksheet.Cells[1, 3].Value = "Имя клиента";
                worksheet.Cells[1, 4].Value = "Отчество клиента";
                worksheet.Cells[1, 5].Value = "Услуга";
                worksheet.Cells[1, 6].Value = "Время посещения";

                // Заполняем таблицу данными из базы данных
                for (int i = 0; i < clientServices.Count; i++)
                {
                    var service = clientServices[i];
                    worksheet.Cells[i + 2, 1].Value = service.ID.ToString(); 
                    worksheet.Cells[i + 2, 2].Value = service.Client.FirstName; 
                    worksheet.Cells[i + 2, 3].Value = service.Client.LastName; 
                    worksheet.Cells[i + 2, 4].Value = service.Client.Patronymic;
                    worksheet.Cells[i + 2, 5].Value = service.Service.Title;
                    worksheet.Cells[i + 2, 6].Value = service.StartTime.ToString();
                }

                // Сохраняем Excel файл в диск
                string filePath = "ClientServicesList.xlsx";
                FileInfo fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);

                // Открываем файл Excel
                Process.Start(filePath);
            }

        }
    }
}
