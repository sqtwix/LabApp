using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;

namespace LabApp.WPF.ViewModels;

public partial class AuditPageViewModel : ObservableObject
{
    private readonly IAdminService _adminService;

    [ObservableProperty]
    private ObservableCollection<PatientsAudit> _patientsAudits = new();

    [ObservableProperty]
    private ObservableCollection<ResultsAudit> _resultsAudits = new();

    [ObservableProperty]
    private int? _filterPatientId;

    [ObservableProperty]
    private DateTime? _filterFrom;

    [ObservableProperty]
    private DateTime? _filterTo;

    [ObservableProperty]
    private int? _filterReferralId;

    [ObservableProperty]
    private int? _filterResearchId;

    public AuditPageViewModel(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [RelayCommand]
    private async Task LoadPatientsAudit()
    {
        var audits = await _adminService.GetPatientsAuditAsync(FilterPatientId, FilterFrom, FilterTo);
        PatientsAudits.Clear();
        foreach (var item in audits)
            PatientsAudits.Add(item);
    }

    [RelayCommand]
    private async Task LoadResultsAudit()
    {
        var audits = await _adminService.GetResultsAuditAsync(FilterReferralId, FilterResearchId);
        ResultsAudits.Clear();
        foreach (var item in audits)
            ResultsAudits.Add(item);
    }
}