using PvReport.Services;
using PvReport.ViewModels;
using System;
using System.Windows;

namespace PvReport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = this;

            InitializeComponent();

            MainViewModel = new MainViewModel();
        }

        public static readonly DependencyProperty MainViewModelProperty = DependencyProperty.Register(
            nameof(MainViewModel), typeof(MainViewModel), typeof(MainWindow), new PropertyMetadata(default(MainViewModel)));

        public MainViewModel MainViewModel
        {
            get => (MainViewModel) GetValue(MainViewModelProperty);
            set => SetValue(MainViewModelProperty, value);
        }

       private void OnTryDirectMailConnection(object sender, RoutedEventArgs e)
        {
            var mailRepository = new MailRepository("imap.gmail.com", 993, true, "xxx@gmail.com", "xxx");
            var allEmails = mailRepository.GetAllMails();

            foreach (var email in allEmails)
            {
                Console.WriteLine(email);
            }
        }
    }
}
