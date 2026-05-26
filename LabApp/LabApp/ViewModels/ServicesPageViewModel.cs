using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using LabApp.WPF.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading.Tasks;
using System;

namespace LabApp.WPF.ViewModels;

public partial class ServicesPageViewModel : ObservableObject
{
    private readonly IServiceService _serviceService;
    private readonly string _userRole;

    [ObservableProperty] private bool _isBusy; // Флаг БД

    [ObservableProperty] private ObservableCollection<Service> _services = new();
    [ObservableProperty] private Service? _selectedService;

    // Списки для ComboBox
    [ObservableProperty] private ObservableCollection<Staff> _staffs = new();
    [ObservableProperty] private ObservableCollection<Research> _researches = new();
    [ObservableProperty] private ObservableCollection<PaymentType> _paymentTypes = new();
    [ObservableProperty] private ObservableCollection<Department> _departments = new();
    [ObservableProperty] private ObservableCollection<Appointment> _appointments = new();

    public bool IsReadOnly => _userRole == "registrar_role";
    public bool CanEdit => !IsReadOnly;

    public ServicesPageViewModel(IServiceService serviceService)
    {
        _serviceService = serviceService;
        _userRole = CurrentUser.Role ?? "registrar_role";
        Task.Run(() => System.Windows.Application.Current.Dispatcher.InvokeAsync(() => LoadServicesCommand.Execute(null)));
    }

    [RelayCommand]
    private async Task LoadServices()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            // Подгружаем справочники
            var staffsList = await _serviceService.GetAllStaffsAsync();
            Staffs.Clear(); foreach (var s in staffsList) Staffs.Add(s);

            var researchesList = await _serviceService.GetAllResearchesAsync();
            Researches.Clear(); foreach (var r in researchesList) Researches.Add(r);

            var paymentTypesList = await _serviceService.GetAllPaymentTypesAsync();
            PaymentTypes.Clear(); foreach (var p in paymentTypesList) PaymentTypes.Add(p);

            var departmentsList = await _serviceService.GetAllDepartmentsAsync();
            Departments.Clear(); foreach (var d in departmentsList) Departments.Add(d);

            var appointmentsList = await _serviceService.GetAllAppointmentsAsync();
            Appointments.Clear(); foreach (var a in appointmentsList) Appointments.Add(a);

            // Подгружаем сами услуги
            var list = await _serviceService.GetAllServicesAsync();
            Services.Clear();
            foreach (var s in list) Services.Add(s);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task SaveServiceAsync(Service service)
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            // Проверка: выбраны ли ключевые поля?
            if (service.StaffId <= 0 || service.ResearchId <= 0 || service.AppointmentId <= 0)
            {
                MessageBox.Show("Для сохранения услуги необходимо выбрать Назначение, Сотрудника и Исследование!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existing = await _serviceService.GetServiceByIdAsync(service.StaffId, service.ResearchId, service.AppointmentId);

            if (existing == null)
            {
                // Записи с таким составным ключом нет в БД -> создаем
                var created = await _serviceService.CreateServiceAsync(service);
                var index = Services.IndexOf(service);
                if (index >= 0) Services[index] = created;
            }
            else
            {
                // Запись существует -> обновляем (дату, тип оплаты, отделение)
                await _serviceService.UpdateServiceAsync(service);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteService(Service service)
    {
        if (service == null) return;

        if (service.StaffId <= 0 || service.ResearchId <= 0 || service.AppointmentId <= 0)
        {
            Services.Remove(service);
            return;
        }

        if (MessageBox.Show($"Удалить услугу?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await _serviceService.DeleteServiceAsync(service.StaffId, service.ResearchId, service.AppointmentId);
                Services.Remove(service);
            }
            finally
            {
                IsBusy = false;
            }
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