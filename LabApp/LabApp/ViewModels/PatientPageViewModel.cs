using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading.Tasks;
using System;

namespace LabApp.WPF.ViewModels;

public partial class PatientPageViewModel : ObservableObject
{
    private readonly IPatientService _patientService;
    private readonly IStaffService _staffService;

    [ObservableProperty]
    private ObservableCollection<Patient> _patients = new();

    [ObservableProperty]
    private ObservableCollection<City> _cities = new();

    [ObservableProperty]
    private Patient? _selectedPatient;

    // Флаг для защиты от двойных вызовов
    [ObservableProperty]
    private bool _isBusy;

    public PatientPageViewModel(IPatientService patientService, IStaffService staffService)
    {
        _patientService = patientService;
        _staffService = staffService;

        // Безопасный вызов загрузки без блокировки потока
        Task.Run(() => System.Windows.Application.Current.Dispatcher.InvokeAsync(() => LoadPatientsCommand.Execute(null)));
    }

    [RelayCommand]
    private async Task LoadPatients()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var list = await _patientService.GetAllPatientsAsync();
            Patients.Clear();
            foreach (var p in list)
                Patients.Add(p);

            var citiesList = await _staffService.GetAllCitiesAsync();
            Cities.Clear();
            foreach (var c in citiesList) Cities.Add(c);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task SavePatientAsync(Patient patient)
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            if (patient.PatientId <= 0) // новая запись
            {
                var created = await _patientService.CreatePatientAsync(patient);
                var index = Patients.IndexOf(patient);
                if (index >= 0)
                    Patients[index] = created;
            }
            else
            {
                await _patientService.UpdatePatientAsync(patient);
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
    private async Task DeletePatient(Patient patient)
    {
        if (patient == null) return;

        if (patient.PatientId <= 0)
        {
            Patients.Remove(patient);
            return;
        }

        if (MessageBox.Show($"Удалить пациента {patient.LastName} {patient.FirstName}?",
                            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            await _patientService.DeletePatientAsync(patient.PatientId);
            Patients.Remove(patient);
        }
    }

    [RelayCommand]
    private void AddPatient()
    {
        var newPatient = new Patient
        {
            PatientId = -1,
            LastName = "Новый",
            FirstName = "Пациент",
            Gender = "М", // Дефолтное значение для избежания null в базе
            BirthDate = new DateOnly(2000, 1, 1)
        };
        Patients.Add(newPatient);
        SelectedPatient = newPatient;
    }
}