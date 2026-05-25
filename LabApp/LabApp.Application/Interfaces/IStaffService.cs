using LabApp.Domain.Entities;

namespace LabApp.Application.Interfaces;

public interface IStaffService
{
    // Сотрудники
    Task<Staff> GetStaffByIdAsync(int id);
    Task<IEnumerable<Staff>> GetAllStaffAsync();
    Task<Staff> CreateStaffAsync(Staff staff);
    Task<Staff> UpdateStaffAsync(Staff staff);
    Task<bool> DeleteStaffAsync(int id);

    Task<IEnumerable<City>> GetAllCitiesAsync();

    // Должности
    Task<IEnumerable<Position>> GetAllPositionsAsync();

    // Отделения (департаменты)
    Task<IEnumerable<Department>> GetAllDepartmentsAsync();
    Task<Department> GetDepartmentByIdAsync(int id);

    // Привязка сотрудника к отделению
    Task AddStaffToDepartmentAsync(int staffId, int departmentId);
    Task RemoveStaffFromDepartmentAsync(int staffId, int departmentId);

    // Специализации
    Task<IEnumerable<Specialization>> GetAllSpecializationsAsync();
    Task AddSpecializationToStaffAsync(int staffId, int specializationId);
    Task RemoveSpecializationFromStaffAsync(int staffId, int specializationId);

    // Расписание
    Task<IEnumerable<StaffSchedule>> GetScheduleForStaffAsync(int staffId);
    Task<StaffSchedule> AddScheduleAsync(StaffSchedule schedule);
    Task<StaffSchedule> UpdateScheduleAsync(StaffSchedule schedule);
    Task<bool> DeleteScheduleAsync(int scheduleId);
}

