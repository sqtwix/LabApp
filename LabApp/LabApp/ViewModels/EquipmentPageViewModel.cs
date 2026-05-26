using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;

namespace LabApp.WPF.ViewModels;

public partial class EquipmentPageViewModel : ObservableObject
{
    private readonly IEquipmentService _equipmentService;

    [ObservableProperty]
    private ObservableCollection<Equipment> _equipmentList = new();

    [ObservableProperty]
    private Equipment? _selectedEquipment;

    public EquipmentPageViewModel(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
        LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadData()
    {
        var list = await _equipmentService.GetAllEquipmentAsync();
        EquipmentList.Clear();
        foreach (var eq in list)
            EquipmentList.Add(eq);
    }

    public async Task SaveEquipmentAsync(Equipment equipment)
    {
        if (equipment.EquipmentId <= 0)
        {
            var created = await _equipmentService.CreateEquipmentAsync(equipment);
            var index = EquipmentList.IndexOf(equipment);
            if (index >= 0)
                EquipmentList[index] = created;
        }
        else
        {
            await _equipmentService.UpdateEquipmentAsync(equipment);
        }
    }

    [RelayCommand]
    private async Task DeleteEquipment(Equipment equipment)
    {
        if (equipment == null) return;
        if (MessageBox.Show($"Удалить оборудование '{equipment.Name}'?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            await _equipmentService.DeleteEquipmentAsync(equipment.EquipmentId);
            EquipmentList.Remove(equipment);
        }
    }

    [RelayCommand]
    private void AddEquipment()
    {
        var newEq = new Equipment
        {
            EquipmentId = -1,
            Name = "Новое оборудование"
        };
        EquipmentList.Add(newEq);
        SelectedEquipment = newEq;
    }
}