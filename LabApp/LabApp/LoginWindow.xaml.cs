using LabApp.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;


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
                // Сохраняем информацию о пользователе
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
