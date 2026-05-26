using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.WPF.Pages;
using LabApp.WPF.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using LabApp.WPF.Utils;

namespace LabApp.WPF.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _userRole;

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
            _userRole = CurrentUser.Role ?? "регистратор";
            BuildMenu();
        }

        private void BuildMenu()
        {
            var patientsItem = new MenuItemViewModel
            {
                Header = "Пациенты",
                Command = new RelayCommand(NavigateToPatients),
                IsVisible = _userRole != "лаборант"
            };
            var staffItem = new MenuItemViewModel
            {
                Header = "Сотрудники",
                Command = new RelayCommand(NavigateToStaff),
                IsVisible = _userRole != "регистратор"
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
                IsVisible = _userRole != "регистратор"
            };
            var reportsItem = new MenuItemViewModel
            {
                Header = "Отчёты",
                Command = new RelayCommand(NavigateToReports),
                IsVisible = _userRole == "админ"
            };
            var auditItem = new MenuItemViewModel
            {
                Header = "Аудит",
                Command = new RelayCommand(NavigateToAudit),
                IsVisible = _userRole == "админ"
            };
            var equipmentItem = new MenuItemViewModel
            {
                Header = "Оборудование",
                Command = new RelayCommand(NavigateToEquipment),
                IsVisible = _userRole != "регистратор" // лаборант и админ видят
            };
            var exitItem = new MenuItemViewModel
            {
                Header = "Выход",
                Command = new RelayCommand(Exit),
                IsVisible = true
            };
            var resultsItem = new MenuItemViewModel
            {
                Header = "Результаты",
                Command = new RelayCommand(NavigateToResults),
                IsVisible = true // или по роли, если нужно
            };

            MenuItems.Add(resultsItem);
            MenuItems.Add(patientsItem);
            MenuItems.Add(staffItem);
            MenuItems.Add(appointmentsItem);
            MenuItems.Add(researchesItem);
            MenuItems.Add(reportsItem);
            MenuItems.Add(exitItem);
            MenuItems.Add(equipmentItem);
            MenuItems.Add(auditItem);

            // Подписка на изменение выбранного элемента
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedMenuItem) && SelectedMenuItem?.Command != null)
                {
                    SelectedMenuItem.Command.Execute(null);
                    SelectedMenuItem = null;
                }
            };
        }

        private void NavigateToPatients()
        {
            CurrentPage = _serviceProvider.GetRequiredService<PatientPage>();
        }

        [RelayCommand]
        private void NavigateToStaff()
        {
            CurrentPage = _serviceProvider.GetRequiredService<StaffPage>();
        }

        [RelayCommand]
        private void NavigateToAppointments()
        {
            CurrentPage = _serviceProvider.GetRequiredService<AppointmentsPage>();
        }

        [RelayCommand]
        private void NavigateToResearches()
        {
            CurrentPage = _serviceProvider.GetRequiredService<ResearchesPage>();
        }

        [RelayCommand]
        private void NavigateToReports()
        {
            CurrentPage = _serviceProvider.GetRequiredService<ReportsPage>();
        }

        [RelayCommand]
        private void NavigateToAudit()
        {
            CurrentPage = _serviceProvider.GetRequiredService<AuditPage>();
        }

        [RelayCommand]
        private void NavigateToEquipment()
        {
            CurrentPage = _serviceProvider.GetRequiredService<EquipmentPage>();
        }

        [RelayCommand]
        private void NavigateToResults()
        {
            CurrentPage = _serviceProvider.GetRequiredService<ResultsPage>();
        }

        [RelayCommand]
        private void SwitchUser()
        {
            // Очищаем данные текущего пользователя
            CurrentUser.StaffId = 0;
            CurrentUser.Role = string.Empty;
            CurrentUser.FullName = string.Empty;

            // Закрываем главное окно
            var mainWindow = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainWindow?.Close();

            // Открываем окно логина
            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        private void Exit()
        {
        }
    }
}