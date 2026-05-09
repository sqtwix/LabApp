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
        var staff = await _context.Staffs
            .FirstOrDefaultAsync(s => s.Login == login);

        if (staff == null || string.IsNullOrEmpty(staff.PasswordHash))
            return (false, null, 0);

        // Проверка пароля (здесь можно использовать BCrypt или другой алгоритм)
        bool valid = VerifyPassword(password, staff.PasswordHash);
        if (!valid) return (false, null, 0);

        return (true, staff.RoleName, staff.StaffId);
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

    // Простая имитация проверки пароля (в реальном проекте используйте BCrypt)
    private bool VerifyPassword(string password, string hash)
    {
        // Заглушка – замените на реальную проверку
        return password == hash;
    }
}