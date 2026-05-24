using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.WPF.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace LabApp.WPF.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _userRole;

        [ObservableProperty]
        private Page? _currentPage;

        [ObservableProperty]
        private MenuItemViewModel? _selectedMenuItem;

        public ObservableCollection<MenuItemViewModel> MenuItems { get; } = new();

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _userRole = CurrentUser.Role ?? "registrar_role";
            BuildMenu();
        }

        private void BuildMenu()
        {
            // Создаём пункты меню с командами и видимостью в зависимости от роли
            var patientsItem = new MenuItemViewModel
            {
                Header = "Пациенты",
                Command = new RelayCommand(NavigateToPatients),
                IsVisible = _userRole != "lab_spec_role"
            };
            var staffItem = new MenuItemViewModel
            {
                Header = "Сотрудники",
                Command = new RelayCommand(NavigateToStaff),
                IsVisible = _userRole != "registrar_role"
            };
            var appointmentsItem = new MenuItemViewModel
            {
                Header = "Назначения",
                Command = new RelayCommand(NavigateToAppointments),
                IsVisible = true // все роли видят назначения
            };
            var reportsItem = new MenuItemViewModel
            {
                Header = "Отчёты",
                Command = new RelayCommand(NavigateToReports),
                IsVisible = _userRole == "administrator_role"
            };
            var exitItem = new MenuItemViewModel
            {
                Header = "Выход",
                Command = new RelayCommand(Exit),
                IsVisible = true
            };

            MenuItems.Add(patientsItem);
            MenuItems.Add(staffItem);
            MenuItems.Add(appointmentsItem);
            MenuItems.Add(reportsItem);
            MenuItems.Add(exitItem);

            // Подписка на изменение выбранного элемента (опционально)
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedMenuItem) && SelectedMenuItem?.Command != null)
                {
                    SelectedMenuItem.Command.Execute(null);
                    SelectedMenuItem = null; // сброс, чтобы можно было повторно выбрать тот же пункт
                }
            };
        }

        private void NavigateToPatients()
        {
            // TODO: заменить на страницу пациентов
            CurrentPage = new Page { Content = new TextBlock { Text = "Пациенты – в разработке" } };
        }

        private void NavigateToStaff()
        {
            CurrentPage = new Page { Content = new TextBlock { Text = "Сотрудники – в разработке" } };
        }

        private void NavigateToAppointments()
        {
            CurrentPage = new Page { Content = new TextBlock { Text = "Назначения – в разработке" } };
        }

        private void NavigateToReports()
        {
            CurrentPage = new Page { Content = new TextBlock { Text = "Отчёты – в разработке" } };
        }

        private void Exit()
        {
            
        }
    }
}