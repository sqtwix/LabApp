using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;

namespace LabApp.WPF.ViewModels;

public partial class StaffPageViewModel : ObservableObject
{
    private readonly IStaffService _staffService;

    [ObservableProperty]
    private ObservableCollection<Staff> _staffList = new();

    [ObservableProperty]
    private Staff? _selectedStaff;

    // Справочники для выпадающих списков (если нужно)
    [ObservableProperty]
    private ObservableCollection<Position> _positions = new();

    [ObservableProperty]
    private ObservableCollection<City> _cities = new();

    public StaffPageViewModel(IStaffService staffService)
    {
        _staffService = staffService;
        LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadData()
    {
        // Загружаем сотрудников
        var staff = await _staffService.GetAllStaffAsync();
        StaffList.Clear();
        foreach (var s in staff)
            StaffList.Add(s);

        // Загружаем справочники (если есть методы)
        if (_staffService.GetAllPositionsAsync != null)
        {
            var positions = await _staffService.GetAllPositionsAsync();
            Positions.Clear();
            foreach (var p in positions)
                Positions.Add(p);
        }
        if (_staffService.GetAllCitiesAsync != null)
        {
            var cities = await _staffService.GetAllCitiesAsync();
            Cities.Clear();
            foreach (var c in cities)
                Cities.Add(c);
        }
    }

    // Сохранение одной записи (после редактирования)
    public async Task SaveStaffAsync(Staff staff)
    {
        if (staff.StaffId <= 0) // новая запись
        {
            var created = await _staffService.CreateStaffAsync(staff);
            var index = StaffList.IndexOf(staff);
            if (index >= 0)
                StaffList[index] = created;
        }
        else
        {
            await _staffService.UpdateStaffAsync(staff);
        }
    }

    [RelayCommand]
    private async Task DeleteStaff(Staff staff)
    {
        if (staff == null) return;
        if (MessageBox.Show($"Удалить сотрудника {staff.LastName} {staff.FirstName}?",
                            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            await _staffService.DeleteStaffAsync(staff.StaffId);
            StaffList.Remove(staff);
        }
    }

    [RelayCommand]
    private void AddStaff()
    {
        var newStaff = new Staff
        {
            StaffId = -1,
            LastName = "Новый",
            FirstName = "Сотрудник"
        };
        StaffList.Add(newStaff);
        SelectedStaff = newStaff;
    }
}