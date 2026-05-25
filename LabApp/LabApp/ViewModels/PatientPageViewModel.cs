using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;

namespace LabApp.WPF.ViewModels;

public partial class PatientPageViewModel : ObservableObject
{
    private readonly IPatientService _patientService;

    [ObservableProperty]
    private ObservableCollection<Patient> _patients = new();

    public PatientPageViewModel(IPatientService patientService)
    {
        _patientService = patientService;
        LoadPatientsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadPatients()
    {
        var list = await _patientService.GetAllPatientsAsync();
        Patients.Clear();
        foreach (var p in list)
            Patients.Add(p);
    }

    // Сохранение одной записи (после редактирования)
    public async Task SavePatientAsync(Patient patient)
    {
        if (patient.PatientId <= 0) // новая запись
        {
            var created = await _patientService.CreatePatientAsync(patient);
            // заменить временный объект на созданный (с реальным ID)
            var index = Patients.IndexOf(patient);
            if (index >= 0)
                Patients[index] = created;
        }
        else
        {
            await _patientService.UpdatePatientAsync(patient);
        }
    }

    // Удаление
    [RelayCommand]
    private async Task DeletePatient(Patient patient)
    {
        if (patient == null) return;
        if (MessageBox.Show($"Удалить пациента {patient.LastName} {patient.FirstName}?",
                            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            await _patientService.DeletePatientAsync(patient.PatientId);
            Patients.Remove(patient);
        }
    }

    // Добавление новой пустой строки
    [RelayCommand]
    private void AddPatient()
    {
        var newPatient = new Patient
        {
            PatientId = -1, // временный ID
            LastName = "Новый",
            FirstName = "Пациент"
        };
        Patients.Add(newPatient);
        // Можно установить выделение на новую строку (через SelectedPatient)
        SelectedPatient = newPatient;
    }

    [ObservableProperty]
    private Patient? _selectedPatient;
}