using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using LabApp.WPF.Utils;
using System.Collections.ObjectModel;
using System.Windows;

namespace LabApp.WPF.ViewModels;

public partial class ServicesPageViewModel : ObservableObject
{
    private readonly IServiceService _serviceService;
    private readonly string _userRole;

    [ObservableProperty]
    private ObservableCollection<Service> _services = new();

    [ObservableProperty]
    private Service? _selectedService;

    public bool IsReadOnly => _userRole == "registrar_role"; // регистратор только читает
    public bool CanEdit => !IsReadOnly;

    public ServicesPageViewModel(IServiceService serviceService)
    {
        _serviceService = serviceService;
        _userRole = CurrentUser.Role ?? "registrar_role";
        LoadServicesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadServices()
    {
        var list = await _serviceService.GetAllServicesAsync();
        Services.Clear();
        foreach (var s in list) Services.Add(s);
    }

    public async Task SaveServiceAsync(Service service)
    {
        // Проверка: новая ли запись (по отрицательным или нулевым значениям составного ключа)
        bool isNew = service.StaffId <= 0 || service.ResearchId <= 0 || service.AppointmentId <= 0;
        if (isNew)
        {
            var created = await _serviceService.CreateServiceAsync(service);
            var index = Services.IndexOf(service);
            if (index >= 0) Services[index] = created;
        }
        else
        {
            await _serviceService.UpdateServiceAsync(service);
        }
    }

    [RelayCommand]
    private async Task DeleteService(Service service)
    {
        if (service == null) return;
        if (MessageBox.Show($"Удалить услугу (сотрудник {service.StaffId}, исследование {service.ResearchId}, запись {service.AppointmentId})?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            await _serviceService.DeleteServiceAsync(service.StaffId, service.ResearchId, service.AppointmentId);
            Services.Remove(service);
        }
    }

    [RelayCommand]
    private void AddService()
    {
        var newService = new Service
        {
            StaffId = -1,
            ResearchId = -1,
            AppointmentId = -1,
            ServiceDate = DateOnly.FromDateTime(DateTime.Today)
        };
        Services.Add(newService);
        SelectedService = newService;
    }
}