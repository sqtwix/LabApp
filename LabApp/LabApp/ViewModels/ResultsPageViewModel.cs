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

public partial class ResultsPageViewModel : ObservableObject
{
    private readonly IResultService _resultService;
    private readonly string _userRole;

    public bool CanEdit => !IsReadOnly;
    public bool IsReadOnly => _userRole == "registrar_role"; // регистратор только читает

    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private ObservableCollection<Result> _results = new();
    [ObservableProperty] private Result? _selectedResult;

    // Коллекции для выпадающих списков
    [ObservableProperty] private ObservableCollection<Appointment> _appointments = new();
    [ObservableProperty] private ObservableCollection<Research> _researches = new();
    [ObservableProperty] private ObservableCollection<ResultCarrierType> _carrierTypes = new();

    public ResultsPageViewModel(IResultService resultService)
    {
        _resultService = resultService;
        _userRole = CurrentUser.Role ?? "registrar_role";

        Task.Run(() => System.Windows.Application.Current.Dispatcher.InvokeAsync(() => LoadResultsCommand.Execute(null)));
    }

    [RelayCommand]
    private async Task LoadResults()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            // Сначала грузим справочники
            var apps = await _resultService.GetAllAppointmentsAsync();
            Appointments.Clear();
            foreach (var a in apps) Appointments.Add(a);

            var res = await _resultService.GetAllResearchesAsync();
            Researches.Clear();
            foreach (var r in res) Researches.Add(r);

            var carriers = await _resultService.GetAllCarrierTypesAsync();
            CarrierTypes.Clear();
            foreach (var c in carriers) CarrierTypes.Add(c);

            // Грузим результаты
            var list = await _resultService.GetAllResultsAsync();
            Results.Clear();
            foreach (var r in list) Results.Add(r);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task SaveResultAsync(Result result)
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            // Определяем, новая ли это запись. Если результат уже есть в коллекции, но был добавлен через кнопку "Добавить"
            // В EF Core изменение составного ключа (ReferralId + ResearchId) у существующей записи невозможно, 
            // поэтому мы просто пытаемся сохранить изменения или создать новую
            var existing = await _resultService.GetResultByIdAsync(result.ReferralId, result.ResearchId);

            if (existing == null)
            {
                var created = await _resultService.CreateResultAsync(result);
                // Заменяем в коллекции
                var index = Results.IndexOf(result);
                if (index >= 0) Results[index] = created;
            }
            else
            {
                await _resultService.UpdateResultAsync(result);
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
    private async Task DeleteResult(Result result)
    {
        if (result == null) return;

        // Если это несохраненная строка (ID пустые)
        if (result.ReferralId <= 0 || result.ResearchId <= 0)
        {
            Results.Remove(result);
            return;
        }

        if (MessageBox.Show($"Удалить результат для назначения №{result.ReferralId}?", "Подтверждение",
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