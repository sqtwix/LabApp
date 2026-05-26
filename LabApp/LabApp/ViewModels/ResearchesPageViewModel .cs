using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading.Tasks;
using System;

namespace LabApp.WPF.ViewModels;

public partial class ResearchesPageViewModel : ObservableObject
{
    private readonly IResearchService _researchService;

    [ObservableProperty] private bool _isBusy; // Замок от двойных вызовов БД

    [ObservableProperty] private ObservableCollection<Research> _researches = new();
    [ObservableProperty] private Research? _selectedResearch;
    [ObservableProperty] private ObservableCollection<ResearchType> _researchTypes = new();

    public ResearchesPageViewModel(IResearchService researchService)
    {
        _researchService = researchService;

        // Загружаем данные без фриза интерфейса
        Task.Run(() => System.Windows.Application.Current.Dispatcher.InvokeAsync(() => LoadDataCommand.Execute(null)));
    }

    [RelayCommand]
    private async Task LoadData()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var types = await _researchService.GetAllResearchTypesAsync();
            ResearchTypes.Clear();
            foreach (var t in types) ResearchTypes.Add(t);

            var list = await _researchService.GetAllResearchesAsync();
            Researches.Clear();
            foreach (var r in list) Researches.Add(r);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task SaveResearchAsync(Research research)
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            if (research.ResearchId <= 0)
            {
                var created = await _researchService.CreateResearchAsync(research);
                var index = Researches.IndexOf(research);
                if (index >= 0)
                    Researches[index] = created;
            }
            else
            {
                var updated = await _researchService.UpdateResearchAsync(research);
                var index = Researches.IndexOf(research);
                if (index >= 0 && updated != null)
                    Researches[index] = updated;
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
    private async Task DeleteResearch(Research research)
    {
        if (research == null) return;

        if (research.ResearchId <= 0)
        {
            Researches.Remove(research);
            return;
        }

        if (MessageBox.Show($"Удалить исследование '{research.Name}'?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await _researchService.DeleteResearchAsync(research.ResearchId);
                Researches.Remove(research);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления (возможно исследование используется): {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    [RelayCommand]
    private void AddResearch()
    {
        var newResearch = new Research
        {
            ResearchId = -1,
            Name = "Новое исследование",
            Cost = 0,
            ResearchTypeId = 2 // Дефолтный тип "Биохимическое исследование"
        };
        Researches.Add(newResearch);
        SelectedResearch = newResearch;
    }
}