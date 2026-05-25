using LabApp.Domain.Entities;
using LabApp.Application.Dtos;

namespace LabApp.Application.Interfaces;

public interface IReportingService
{
    Task<decimal> GetProfitAsync(int labId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<DepartmentStaffCountDto>> GetDepartmentsWithStaffCountAsync();
    Task<IEnumerable<Patient>> GetPensionPatientsAsync(); // если есть в IPatientService, можно взять оттуда
}