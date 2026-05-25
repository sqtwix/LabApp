using LabApp.Application.Dtos;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using LabApp.Infrustructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabApp.Infrastructure.Services;

public class ReportingService : IReportingService
{
    private readonly LabContext _context;

    public ReportingService(LabContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetProfitAsync(int labId, DateTime startDate, DateTime endDate)
    {
        var startStr = startDate.ToString("yyyy-MM-dd");
        var endStr = endDate.ToString("yyyy-MM-dd");
        var sql = $"SELECT * FROM get_profit({labId}, '{startStr}'::date, '{endStr}'::date)";

        var result = await _context.Database
            .SqlQueryRaw<ProfitResult>(sql)
            .FirstOrDefaultAsync();

        return result?.profit ?? 0;
    }

    public async Task<IEnumerable<DepartmentStaffCountDto>> GetDepartmentsWithStaffCountAsync()
    {
        var query = await _context.GetDeprtmentsWithStaffCounts.ToListAsync();
        return query.Select(item => new DepartmentStaffCountDto
        {
            DepartmentId = item.IdОтделения ?? 0,          // int? → int
            DepartmentName = item.НазваниеОтделения ?? "",
            StaffCount = (int)(item.КолВоСотурдниковВОтделении ?? 0) // long? → int
        });
    }

    public async Task<IEnumerable<Patient>> GetPensionPatientsAsync()
    {
        var pensionView = await _context.GetPensionPatients.ToListAsync();
        var patients = pensionView.Select(p => new Patient
        {
            PatientId = p.IdПациента ?? 0,
            // ФИО может быть в формате "Фамилия Имя Отчество". Разбиваем.
            LastName = p.Фио?.Split(' ').FirstOrDefault() ?? "",
            FirstName = p.Фио?.Split(' ').Skip(1).FirstOrDefault() ?? "",
            MiddleName = p.Фио?.Split(' ').Skip(2).FirstOrDefault(),
            Gender = p.Пол
        });
        return patients;
    }
    private class ProfitResult
    {
        public string lab_id { get; set; }    // точно как в SQL
        public decimal profit { get; set; }   // точно как в SQL
    }
}
