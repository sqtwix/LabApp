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

    [ObservableProperty]
    private ObservableCollection<string> _roles = new()
    {
        "administrator_role",
        "lab_spec_role",
        "registrar_role"
    };

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
        try
        {
            if (staff.StaffId <= 0)
            {
                // Новая запись
                if (string.IsNullOrWhiteSpace(staff.Login))
                    staff.Login = "temp_" + Guid.NewGuid().ToString("N").Substring(0, 8);
                if (string.IsNullOrWhiteSpace(staff.RoleName))
                    staff.RoleName = "registrar_role";
                if (staff.Passport == null) staff.Passport = "";
                if (staff.Address == null) staff.Address = "";
                staff.PasswordHash = "temp123";

                var created = await _staffService.CreateStaffAsync(staff);
                // Перезагружаем из БД, чтобы получить полные данные (включая сгенерированный ID и хэш)
                var fresh = await _staffService.GetStaffByIdAsync(created.StaffId);
                var index = Staffs.IndexOf(staff);
                if (index >= 0)
                    Staffs[index] = fresh;
                else
                    Staffs.Add(fresh);
            }
            else
            {
                // Обновление существующей записи
                await _staffService.UpdateStaffAsync(staff);
                // Обновляем локальный объект (данные уже обновлены в staff)
                var index = Staffs.IndexOf(staff);
                if (index >= 0)
                    Staffs[index] = staff;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task DeleteStaff(Staff staff)
    {
        if (staff == null) return;
        if (staff.StaffId <= 0)
        {
            Staffs.Remove(staff);
            return;
        }
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
            LastName = "Новый",
            FirstName = "Сотрудник",
            Login = "temp_" + Guid.NewGuid().ToString("N").Substring(0, 8),
            RoleName = "registrar_role",
            Passport = "",
            Address = "",
            Phone = "",
            Education = ""
        };
        Staffs.Add(newStaff);
        SelectedStaff = newStaff;
    }
}