using LabApp.Domain.Entities;


namespace LabApp.Application.Interfaces;

public interface IAdminService
{
    // Аутентификация и роли (если реализовано)
    Task<(bool Success, string Role, int StaffId)> AuthenticateAsync(string login, string password);
    Task<bool> ChangeStaffRoleAsync(int staffId, string newRole);

    // Аудит изменений
    Task<IEnumerable<PatientsAudit>> GetPatientsAuditAsync(int? patientId = null, DateTime? from = null, DateTime? to = null);
    Task<IEnumerable<ResultsAudit>> GetResultsAuditAsync(int? referralId = null, int? researchId = null);
}

