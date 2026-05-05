using LabApp.Domain.Entities;

namespace LabApp.Application.Interfaces;

public interface IPatientService
{
    // Основные CRUD
    Task<Patient> GetPatientByIdAsync(int id);
    Task<IEnumerable<Patient>> GetAllPatientsAsync();
    Task<Patient> CreatePatientAsync(Patient patient);
    Task<Patient> UpdatePatientAsync(Patient patient);
    Task<bool> DeletePatientAsync(int id);

    // Страховки
    Task<IEnumerable<InsuranceCompany>> GetPatientInsurancesAsync(int patientId);
    Task AddInsuranceToPatientAsync(int patientId, int insuranceId);
    Task RemoveInsuranceFromPatientAsync(int patientId, int insuranceId);

    // Представления и отчёты
    Task<IEnumerable<Patient>> GetPensionPatientsAsync();          // пенсионный возраст
    Task<IEnumerable<PatientWithAnalyzesDto>> GetPatientsWithAnalyzesAsync(); // частота обращений
}


