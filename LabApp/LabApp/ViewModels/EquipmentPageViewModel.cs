using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading.Tasks;
using System;

namespace LabApp.WPF.ViewModels;

public partial class EquipmentPageViewModel : ObservableObject
{
    private readonly IEquipmentService _equipmentService;

    [ObservableProperty] private bool _isBusy; // Флаг блокировки БД

    [ObservableProperty] private ObservableCollection<Equipment> _equipmentList = new();
    [ObservableProperty] private Equipment? _selectedEquipment;
    [ObservableProperty] private ObservableCollection<Room> _rooms = new();

    public EquipmentPageViewModel(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
        // Безопасный запуск загрузки
        Task.Run(() => System.Windows.Application.Current.Dispatcher.InvokeAsync(() => LoadDataCommand.Execute(null)));
    }

    [RelayCommand]
    private async Task LoadData()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            // Подгружаем кабинеты для выпадающего списка!
            var roomList = await _equipmentService.GetAllRoomsAsync();
            Rooms.Clear();
            foreach (var r in roomList) Rooms.Add(r);

            var list = await _equipmentService.GetAllEquipmentAsync();
            EquipmentList.Clear();
            foreach (var eq in list) EquipmentList.Add(eq);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task SaveEquipmentAsync(Equipment equipment)
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            if (equipment.EquipmentId <= 0)
            {
                var created = await _equipmentService.CreateEquipmentAsync(equipment);
                var index = EquipmentList.IndexOf(equipment);
                if (index >= 0) EquipmentList[index] = created;
            }
            else
            {
                var updated = await _equipmentService.UpdateEquipmentAsync(equipment);
                var index = EquipmentList.IndexOf(equipment);
                if (index >= 0 && updated != null) EquipmentList[index] = updated;
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
    private async Task DeleteEquipment(Equipment equipment)
    {
        if (equipment == null) return;

        if (equipment.EquipmentId <= 0)
        {
            EquipmentList.Remove(equipment);
            return;
        }

        if (MessageBox.Show($"Удалить оборудование '{equipment.Name}'?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await _equipmentService.DeleteEquipmentAsync(equipment.EquipmentId);
                EquipmentList.Remove(equipment);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    [RelayCommand]
    private void AddEquipment()
    {
        var newEq = new Equipment
        {
            EquipmentId = -1,
            Name = "Новое оборудование",
            ExplotationDate = DateOnly.FromDateTime(DateTime.Today),
            ServiceLife = 5 // По умолчанию
        };
        EquipmentList.Add(newEq);
        SelectedEquipment = newEq;
    }
}