using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Dtos;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;

namespace LabApp.WPF.ViewModels;

public partial class AppointmentsPageViewModel : ObservableObject
{
    private readonly IAppointmentService _appointmentService;

    [ObservableProperty]
    private ObservableCollection<Appointment> _appointments = new();

    [ObservableProperty]
    private Appointment? _selectedAppointment;

    [ObservableProperty]
    private ObservableCollection<Patient> _patients = new();

    [ObservableProperty]
    private ObservableCollection<Staff> _staffs = new();

    public List<string> Statuses { get; } = new() { "Запланирована", "Завершена", "Не пришёл" };

    public AppointmentsPageViewModel(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
        LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadData()
    {
        var list = await _appointmentService.GetAllAppointmentsAsync();
        Appointments.Clear();
        foreach (var a in list) Appointments.Add(a);

        var patientsList = await _appointmentService.GetAllPatientsAsync();
        Patients.Clear();
        foreach (var p in patientsList) Patients.Add(p);

        var staffsList = await _appointmentService.GetAllStaffAsync();
        Staffs.Clear();
        foreach (var s in staffsList) Staffs.Add(s);
    }

    public async Task SaveAppointmentAsync(Appointment appointment)
    {
        if (appointment.AppointmentId <= 0)
        {
            var dto = new CreateAppointmentDto
            {
                StaffId = appointment.StaffId,
                PatientId = appointment.PatientId,
                AppointmentDate = appointment.AppointmentDate.HasValue
                    ? appointment.AppointmentDate.Value.ToDateTime(TimeOnly.MinValue)
                    : null,
                AppointmentTime = appointment.AppointmentTime.HasValue
                    ? appointment.AppointmentTime.Value.ToTimeSpan()
                    : null,
                Status = appointment.Status
            };
            var created = await _appointmentService.CreateAppointmentAsync(dto);
            var index = Appointments.IndexOf(appointment);
            if (index >= 0)
                Appointments[index] = created;
        }
        else
        {
            await _appointmentService.UpdateAppointmentAsync(appointment);
        }
    }

    [RelayCommand]
    private async Task DeleteAppointment(Appointment appointment)
    {
        if (appointment == null) return;
        if (MessageBox.Show($"Удалить запись #{appointment.AppointmentId}?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            await _appointmentService.DeleteAppointmentAsync(appointment.AppointmentId);
            Appointments.Remove(appointment);
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