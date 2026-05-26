using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.WPF.Pages;
using LabApp.WPF.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace LabApp.WPF.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    public string? UserRole => CurrentUser.Role;
    public string? UserFullName => CurrentUser.FullName;

    [ObservableProperty]
    private Page? _currentPage;

    [ObservableProperty]
    private MenuItemViewModel? _selectedMenuItem;

    public ObservableCollection<MenuItemViewModel> MenuItems { get; } = new();

    public MainWindowViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        BuildMenu();
    }

    private void BuildMenu()
    {
        var patientsItem = new MenuItemViewModel
        {
            Header = "Пациенты",
            Command = new RelayCommand(NavigateToPatients),
            IsVisible = CurrentUser.Role != "лаборант"
        };
        var staffItem = new MenuItemViewModel
        {
            Header = "Сотрудники",
            Command = new RelayCommand(NavigateToStaff),
            IsVisible = CurrentUser.Role != "регистратор"
        };
        var appointmentsItem = new MenuItemViewModel
        {
            Header = "Назначения",
            Command = new RelayCommand(NavigateToAppointments),
            IsVisible = true
        };
        var researchesItem = new MenuItemViewModel
        {
            Header = "Исследования",
            Command = new RelayCommand(NavigateToResearches),
            IsVisible = CurrentUser.Role != "регистратор"
        };
        var equipmentItem = new MenuItemViewModel
        {
            Header = "Оборудование",
            Command = new RelayCommand(NavigateToEquipment),
            IsVisible = CurrentUser.Role != "регистратор"
        };
        var resultsItem = new MenuItemViewModel
        {
            Header = "Результаты",
            Command = new RelayCommand(NavigateToResults),
            IsVisible = true
        };
        var servicesItem = new MenuItemViewModel
        {
            Header = "Услуги",
            Command = new RelayCommand(NavigateToServices),
            IsVisible = true
        };
        var reportsItem = new MenuItemViewModel
        {
            Header = "Отчёты",
            Command = new RelayCommand(NavigateToReports),
            IsVisible = CurrentUser.Role == "админ"
        };
        var auditItem = new MenuItemViewModel
        {
            Header = "Аудит",
            Command = new RelayCommand(NavigateToAudit),
            IsVisible = CurrentUser.Role == "админ"
        };

        MenuItems.Add(patientsItem);
        MenuItems.Add(staffItem);
        MenuItems.Add(appointmentsItem);
        MenuItems.Add(researchesItem);
        MenuItems.Add(equipmentItem);
        MenuItems.Add(resultsItem);
        MenuItems.Add(servicesItem);
        MenuItems.Add(reportsItem);
        MenuItems.Add(auditItem);
    }

            private void NavigateToPatients()
    {
        CurrentPage = _serviceProvider.GetRequiredService<PatientPage>();
    }

    private void NavigateToStaff()
    {
        CurrentPage = _serviceProvider.GetRequiredService<StaffPage>();
    }

    private void NavigateToAppointments()
    {
        CurrentPage = _serviceProvider.GetRequiredService<AppointmentsPage>();
    }

    private void NavigateToResearches()
    {
        CurrentPage = _serviceProvider.GetRequiredService<ResearchesPage>();
    }

    private void NavigateToEquipment()
    {
        CurrentPage = _serviceProvider.GetRequiredService<EquipmentPage>();
    }

    private void NavigateToResults()
    {
        CurrentPage = _serviceProvider.GetRequiredService<ResultsPage>();
    }

    private void NavigateToServices()
    {
        CurrentPage = _serviceProvider.GetRequiredService<ServicesPage>();
    }

    private void NavigateToReports()
    {
        CurrentPage = _serviceProvider.GetRequiredService<ReportsPage>();
    }

    private void NavigateToAudit()
    {
        CurrentPage = _serviceProvider.GetRequiredService<AuditPage>();
    }

    [RelayCommand]
    private void SwitchUser()
    {
        var result = MessageBox.Show(
            "Вы действительно хотите сменить пользователя?\nНесохранённые данные будут потеряны.",
            "Подтверждение смены пользователя",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        // Очищаем данные текущего пользователя
        CurrentUser.StaffId = 0;
        CurrentUser.Role = string.Empty;
        CurrentUser.FullName = string.Empty;

        // Закрываем главное окно
        var mainWindow = App.Current.Windows.OfType<MainWindow>().FirstOrDefault();
        if (mainWindow != null)
            mainWindow.Visibility = Visibility.Hidden; // Скрываем, чтобы не мешалось

        // Открываем окно авторизации
        var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
        loginWindow.Closed += (s, e) =>
        {
            // Если пользователь не авторизовался (окно закрыто без входа), выходим из приложения
            if (!CurrentUser.IsAuthenticated)
                App.Current.Shutdown();
            else
                mainWindow?.Close(); // Закрываем скрытое окно после успешного входа
        };
        loginWindow.Show();

        // Закрываем текущее главное окно, если оно было открыто
        mainWindow?.Close();
    }

    [RelayCommand]
    private void ExitApp()
    {
        var result = MessageBox.Show(
            "Вы действительно хотите выйти из приложения?",
            "Подтверждение выхода",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            App.Current.Shutdown();
        }
    }

    [RelayCommand]
    private void ShowInfo()
    {
        MessageBox.Show(
            "Медицинская лаборатория\nВерсия 1.0\nРазработано в рамках курсового проекта",
            "О программе",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}