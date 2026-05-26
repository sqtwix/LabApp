using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using LabApp.WPF.Utils;
using System.Collections.ObjectModel;
using System.Windows;

namespace LabApp.WPF.ViewModels;

public partial class ResultsPageViewModel : ObservableObject
{
    private readonly IResultService _resultService;
    private readonly string _userRole;

    public bool CanEdit => !IsReadOnly;

    [ObservableProperty]
    private ObservableCollection<Result> _results = new();

    [ObservableProperty]
    private Result? _selectedResult;

    public bool IsReadOnly => _userRole == "registrar_role"; // регистратор только читает

    public ResultsPageViewModel(IResultService resultService)
    {
        _resultService = resultService;
        _userRole = CurrentUser.Role ?? "registrar_role";
        LoadResultsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadResults()
    {
        var list = await _resultService.GetAllResultsAsync();
        Results.Clear();
        foreach (var r in list) Results.Add(r);
    }

    public async Task SaveResultAsync(Result result)
    {
        if (result.ReferralId <= 0 || result.ResearchId <= 0)
        {
            // Новая запись (нужно, чтобы ReferralId и ResearchId были заполнены)
            var created = await _resultService.CreateResultAsync(result);
            var index = Results.IndexOf(result);
            if (index >= 0) Results[index] = created;
        }
        else
        {
            await _resultService.UpdateResultAsync(result);
        }
    }

    [RelayCommand]
    private async Task DeleteResult(Result result)
    {
        if (result == null) return;
        if (MessageBox.Show($"Удалить результат для referral {result.ReferralId}, research {result.ResearchId}?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            await _resultService.DeleteResultAsync(result.ReferralId, result.ResearchId);
            Results.Remove(result);
        }
    }

    [RelayCommand]
    private void AddResult()
    {
        var newResult = new Result
        {
            ReferralId = -1,
            ResearchId = -1,
            Description = "Новый результат",
            CompletionDate = DateOnly.FromDateTime(DateTime.Today)
        };
        Results.Add(newResult);
        SelectedResult = newResult;
    }
}