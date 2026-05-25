using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Interfaces;
using LabApp.Application.Dtos;  
using LabApp.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;

namespace LabApp.WPF.ViewModels;

public partial class ReportsPageViewModel : ObservableObject
{
    private readonly IReportingService _reportingService;

    [ObservableProperty]
    private DateTime? _startDate;

    [ObservableProperty]
    private DateTime? _endDate;

    [ObservableProperty]
    private decimal _profit;

    [ObservableProperty]
    private ObservableCollection<DepartmentStaffCountDto> _departments = new();

    [ObservableProperty]
    private ObservableCollection<Patient> _pensionPatients = new();

    public ReportsPageViewModel(IReportingService reportingService)
    {
        _reportingService = reportingService;
        StartDate = DateTime.Today.AddMonths(-1);
        EndDate = DateTime.Today;
    }

    [RelayCommand]
    private async Task LoadProfit()
    {
        if (StartDate == null || EndDate == null)
        {
            MessageBox.Show("Выберите период");
            return;
        }
        // labId = 1 – можно вынести в конфигурацию или передавать параметром
        Profit = await _reportingService.GetProfitAsync(1, StartDate.Value, EndDate.Value);
    }

    [RelayCommand]
    private async Task LoadDepartments()
    {
        var depts = await _reportingService.GetDepartmentsWithStaffCountAsync();
        Departments.Clear();
        foreach (var d in depts)
            Departments.Add(d);
    }

    [RelayCommand]
    private async Task LoadPensionPatients()
    {
        var patients = await _reportingService.GetPensionPatientsAsync();
        PensionPatients.Clear();
        foreach (var p in patients)
            PensionPatients.Add(p);
    }
}