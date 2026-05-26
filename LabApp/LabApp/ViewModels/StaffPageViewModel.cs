using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using LabApp.WPF.Utils;
using System.Collections.ObjectModel;
using System.Windows;

namespace LabApp.WPF.ViewModels;

public partial class StaffPageViewModel : ObservableObject
{
    private readonly IStaffService _staffService;

    [ObservableProperty]
    private ObservableCollection<Staff> _staffs = new();

    [ObservableProperty]
    private Staff? _selectedStaff;

    [ObservableProperty]
    private ObservableCollection<City> _cities = new();

    [ObservableProperty]
    private ObservableCollection<Position> _positions = new();

    public bool CanEdit => !IsReadOnly;

    public bool IsReadOnly => CurrentUser.Role == "лаборант";

    public StaffPageViewModel(IStaffService staffService)
    {
        _staffService = staffService;
        LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadData()
    {
        var staff = await _staffService.GetAllStaffAsync();
        Staffs.Clear();
        foreach (var s in staff) Staffs.Add(s);

        var positions = await _staffService.GetAllPositionsAsync();
        Positions.Clear();
        foreach (var p in positions) Positions.Add(p);

        var cities = await _staffService.GetAllCitiesAsync();
        Cities.Clear();
        foreach (var c in cities) Cities.Add(c);
    }

    public async Task SaveStaffAsync(Staff staff)
    {
        if (staff.StaffId <= 0)
        {
            var created = await _staffService.CreateStaffAsync(staff);
            var index = Staffs.IndexOf(staff);
            if (index >= 0) Staffs[index] = created;
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
            Staffs.Remove(staff);
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
        Staffs.Add(newStaff);
        SelectedStaff = newStaff;
    }
}