using LabApp.Application.Interfaces;
using LabApp.Infrustructure;
using Microsoft.EntityFrameworkCore;
using LabApp.Application.Dtos;

namespace LabApp.Infrastructure.Services;

public class ReportingService : IReportingService
{
    private readonly LabContext _context;

    public ReportingService(LabContext context)
    {
        _context = context;
    }

    public async Task<(string LabId, decimal Profit)> GetProfitAsync(int labId, DateTime? startDate, DateTime? endDate)
    {
        // Вызов функции get_profit из базы
        var result = await _context.Set<ProfitResult>()
            .FromSqlRaw("SELECT * FROM get_profit({0}, {1}, {2})", labId, startDate, endDate)
            .ToListAsync();
        var row = result.FirstOrDefault();
        return (row?.LabId ?? labId.ToString(), row?.Profit ?? 0);
    }

    public async Task<IEnumerable<DepartmentStaffCountDto>> GetDepartmentsWithStaffCountAsync()
    {
        // Используем представление get_deprtments_with_staff_count
        var query = await _context.Departments
            .Join(_context.StaffDepartments, d => d.DepartmentId, sd => sd.DepartmentId, (d, sd) => new { d, sd })
            .Join(_context.Staffs, ds => ds.sd.StaffId, s => s.StaffId, (ds, s) => new { ds.d, s })
            .GroupBy(x => new { x.d.DepartmentId, x.d.Name })
            .Select(g => new DepartmentStaffCountDto
            {
                DepartmentId = g.Key.DepartmentId,
                DepartmentName = g.Key.Name,
                StaffCount = g.Count()
            })
            .ToListAsync();
        return query;
    }
}

// Вспомогательный класс для результата profit
internal class ProfitResult
{
    public string LabId { get; set; }
    public decimal Profit { get; set; }
}
