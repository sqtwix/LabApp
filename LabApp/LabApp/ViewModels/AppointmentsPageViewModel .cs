using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Dtos;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading.Tasks;
using System;

namespace LabApp.WPF.ViewModels;

public partial class AppointmentsPageViewModel : ObservableObject
{
    private readonly IAppointmentService _appointmentService;
    private readonly IResearchService _researchService;

    [ObservableProperty] private bool _isBusy; // ГЛАВНЫЙ ЗАМОК БД

    [ObservableProperty] private ObservableCollection<Appointment> _appointments = new();
    [ObservableProperty] private Appointment? _selectedAppointment;
    [ObservableProperty] private ObservableCollection<Patient> _patients = new();
    [ObservableProperty] private ObservableCollection<Staff> _staffs = new();
    [ObservableProperty] private ObservableCollection<Result> _selectedAppointmentResults = new();

    public List<string> Statuses { get; } = new() { "Запланирована", "Завершена", "Не пришёл" };

    public AppointmentsPageViewModel(IAppointmentService appointmentService, IResearchService researchService)
    {
        _appointmentService = appointmentService;
        _researchService = researchService;

        // Асинхронная загрузка при старте
        Task.Run(() => System.Windows.Application.Current.Dispatcher.InvokeAsync(() => LoadDataCommand.Execute(null)));
    }

    [RelayCommand]
    private async Task LoadData()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var patientsList = await _appointmentService.GetAllPatientsAsync();
            Patients.Clear();
            foreach (var p in patientsList) Patients.Add(p);

            var staffsList = await _appointmentService.GetAllStaffAsync();
            Staffs.Clear();
            foreach (var s in staffsList) Staffs.Add(s);

            var list = await _appointmentService.GetAllAppointmentsAsync();
            Appointments.Clear();
            foreach (var a in list) Appointments.Add(a);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task SaveAppointmentAsync(Appointment appointment)
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            if (appointment.AppointmentId <= 0)
            {
                var dto = new CreateAppointmentDto
                {
                    StaffId = appointment.StaffId,
                    PatientId = appointment.PatientId,
                    AppointmentDate = appointment.AppointmentDate?.ToDateTime(TimeOnly.MinValue),
                    AppointmentTime = appointment.AppointmentTime?.ToTimeSpan(),
                    Status = appointment.Status
                };

                var created = await _appointmentService.CreateAppointmentAsync(dto);
                var index = Appointments.IndexOf(appointment);
                if (index >= 0) Appointments[index] = created;
            }
            else
            {
                var updated = await _appointmentService.UpdateAppointmentAsync(appointment);
                var index = Appointments.IndexOf(appointment);
                // Обновляем ссылку в коллекции, чтобы UI подхватил имена
                if (index >= 0 && updated != null) Appointments[index] = updated;
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
    private async Task DeleteAppointment(Appointment appointment)
    {
        if (appointment == null) return;
        if (appointment.AppointmentId <= 0)
        {
            Appointments.Remove(appointment);
            return;
        }

        if (MessageBox.Show($"Удалить запись #{appointment.AppointmentId}?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await _appointmentService.DeleteAppointmentAsync(appointment.AppointmentId);
                Appointments.Remove(appointment);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    [RelayCommand]
    private async Task LoadResultsForAppointment()
    {
        if (SelectedAppointment == null || SelectedAppointment.AppointmentId <= 0) return;

        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var results = await _researchService.GetResultByReferralAndResearchAsync(SelectedAppointment.AppointmentId, 0);
            SelectedAppointmentResults.Clear();
            if (results != null)
                SelectedAppointmentResults.Add(results);
        }
        catch { /* Игнорируем ошибки подгрузки результатов */ }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task UpdateStatusAsync(Appointment appointment, bool isMissed)
    {
        if (appointment == null || appointment.AppointmentId <= 0) return;

        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var (message, updated) = await _appointmentService.UpdateAppointmentStatusAsync(appointment.AppointmentId, isMissed);
            if (updated != null)
            {
                var index = Appointments.IndexOf(appointment);
                if (index >= 0) Appointments[index] = updated;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка изменения статуса: {ex.InnerException?.Message ?? ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void AddAppointment()
    {
        var newAppointment = new Appointment
        {
            AppointmentId = -1,
            AppointmentDate = DateOnly.FromDateTime(DateTime.Today),
            AppointmentTime = TimeOnly.FromDateTime(DateTime.Now),
            Status = "Запланирована"
        };
        Appointments.Add(newAppointment);
        SelectedAppointment = newAppointment;
    }
}