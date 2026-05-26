using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;

namespace LabApp.WPF.ViewModels;

public partial class ResearchesPageViewModel : ObservableObject
{
    private readonly IResearchService _researchService;

    [ObservableProperty]
    private ObservableCollection<Research> _researches = new();

    [ObservableProperty]
    private Research? _selectedResearch;

    [ObservableProperty]
    private ObservableCollection<ResearchType> _researchTypes = new(); 

    public ResearchesPageViewModel(IResearchService researchService)
    {
        _researchService = researchService;
        LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadData()
    {
        var list = await _researchService.GetAllResearchesAsync();
        Researches.Clear();
        foreach (var r in list) Researches.Add(r);

        var types = await _researchService.GetAllResearchTypesAsync();
        ResearchTypes.Clear();
        foreach (var t in types) ResearchTypes.Add(t);
    }

    public async Task SaveResearchAsync(Research research)
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
            await _researchService.UpdateResearchAsync(research);
        }
    }

    [RelayCommand]
    private async Task DeleteResearch(Research research)
    {
        if (research == null) return;
        if (MessageBox.Show($"Удалить исследование '{research.Name}'?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            await _researchService.DeleteResearchAsync(research.ResearchId);
            Researches.Remove(research);
        }
    }

    [RelayCommand]
    private void AddResearch()
    {
        var newResearch = new Research
        {
            ResearchId = -1,
            Name = "Новое исследование"
        };
        Researches.Add(newResearch);
        SelectedResearch = newResearch;
    }
}