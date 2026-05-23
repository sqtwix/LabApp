using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using LabApp.Infrustructure;
using Microsoft.EntityFrameworkCore;

namespace LabApp.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly LabContext _context;

    public AdminService(LabContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Role, int StaffId)> AuthenticateAsync(string login, string password)
    {
        var sql = "SELECT * FROM authenticate_staff({0}, {1})";
        var result = await _context.Staffs
            .FromSqlRaw(sql, login, password)
            .Select(s => new { s.StaffId, s.RoleName })
            .FirstOrDefaultAsync();

        if (result == null)
            return (false, null, 0);

        return (true, result.RoleName, result.StaffId);
    }

    public async Task<Staff> CreateStaffAsync(Staff staff, string plainPassword)
    {
        var sql = @"
            SELECT add_staff(
                {0}, {1}, {2}, {3}, {4}, {5}, {6},
                {7}, {8}, {9}, {10}, {11}, {12}, {13}
            ) AS staff_id";

        var staffId = await _context.Database
            .SqlQueryRaw<int>(sql,
                staff.LastName, staff.FirstName, staff.MiddleName,
                staff.Gender, staff.Phone, staff.Passport, staff.Address,
                staff.BirthDate, staff.PositionId, staff.CityId, staff.Education,
                staff.Login, plainPassword, staff.RoleName)
            .FirstOrDefaultAsync();

        staff.StaffId = staffId;
        return staff;
    }

    public async Task<bool> ChangeStaffRoleAsync(int staffId, string newRole)
    {
        var staff = await _context.Staffs.FindAsync(staffId);
        if (staff == null) return false;
        staff.RoleName = newRole;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PatientsAudit>> GetPatientsAuditAsync(int? patientId = null, DateTime? from = null, DateTime? to = null)
    {
        var query = _context.PatientsAudits.AsQueryable();
        if (patientId.HasValue)
            query = query.Where(a => a.PatientId == patientId.Value);
        if (from.HasValue)
            query = query.Where(a => a.ChangedAt >= from.Value);
        if (to.HasValue)
            query = query.Where(a => a.ChangedAt <= to.Value);
        return await query.OrderByDescending(a => a.ChangedAt).ToListAsync();
    }

    public async Task<IEnumerable<ResultsAudit>> GetResultsAuditAsync(int? referralId = null, int? researchId = null)
    {
        var query = _context.ResultsAudits.AsQueryable();
        if (referralId.HasValue)
            query = query.Where(a => a.ReferralId == referralId.Value);
        if (researchId.HasValue)
            query = query.Where(a => a.ResearchId == researchId.Value);
        return await query.OrderByDescending(a => a.ChangedAt).ToListAsync();
    }
}