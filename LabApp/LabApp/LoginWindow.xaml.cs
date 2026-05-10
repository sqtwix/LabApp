using LabApp.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
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

namespace LabApp.WPF
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IAdminService _adminService;
        private readonly IServiceProvider _serviceProvider;

        public LoginWindow(IAdminService adminService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _adminService = adminService;
            _serviceProvider = serviceProvider;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text;
            var password = PasswordBox.Password;

            var (success, role, staffId) = await _adminService.AuthenticateAsync(login, password);
            if (success)
            {
                // Сохраняем информацию о пользователе (можно в статический класс или в App)
                App.Current.Properties["StaffId"] = staffId;
                App.Current.Properties["Role"] = role;

                // Открываем главное окно
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();

                this.Close(); // закрываем окно входа
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
