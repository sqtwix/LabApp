using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace LabApp
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _userRole;

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _userRole = App.Current.Properties["Role"]?.ToString() ?? "";

            ConfigureMenuVisibility();
        }

        private void ConfigureMenuVisibility()
        {
            // Администратор – всё видно
            if (_userRole == "lab_spec_role")
            {
                PatientsMenuItem.Visibility = Visibility.Collapsed;
                ReportsMenuItem.Visibility = Visibility.Collapsed;
            }
            else if (_userRole == "registrar_role")
            {
                StaffMenuItem.Visibility = Visibility.Collapsed;
                ReportsMenuItem.Visibility = Visibility.Collapsed;
            }
            // Для administrator_role ничего не скрываем
        }

        private void PatientsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Временно показываем заглушку, потом замените на реальную страницу
            ContentFrame.Navigate(new TextBlock { Text = "Страница пациентов (в разработке)", FontSize = 20 });
        }

        private void StaffMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new TextBlock { Text = "Страница сотрудников (в разработке)", FontSize = 20 });
        }

        private void AppointmentsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new TextBlock { Text = "Страница назначений (в разработке)", FontSize = 20 });
        }

        private void ReportsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new TextBlock { Text = "Страница отчётов (в разработке)", FontSize = 20 });
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}