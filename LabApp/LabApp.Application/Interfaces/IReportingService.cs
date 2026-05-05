using LabApp.Application.Dtos;

namespace LabApp.Application.Interfaces;

public interface IReportingService
{
    // Доход лаборатории (хранимая функция get_profit)
    Task<(string LabId, decimal Profit)> GetProfitAsync(int labId, DateTime? startDate, DateTime? endDate);

    // Количество сотрудников по отделениям (представление)
    Task<IEnumerable<DepartmentStaffCountDto>> GetDepartmentsWithStaffCountAsync();

    // Другие сводные отчёты (при необходимости)
    // Например, загрузка отделений, статистика по услугам и т.п.
}

