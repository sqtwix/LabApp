using LabApp.Application.Dtos;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using LabApp.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace LabApp.Infrustructure.Services;

public class PatientService : IPatientService
{
    private readonly LabContext _context;

    public PatientService(LabContext context)
    {
        _context = context;
    }

    public async Task<Patient> GetPatientByIdAsync(int id)
    {
        return await _context.Patients
            .Include(p => p.City)
            .FirstOrDefaultAsync(p => p.PatientId == id);
    }

    public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
    {
        return await _context.Patients
            .Include(p => p.City)
            .ToListAsync();
    }

    public async Task<Patient> CreatePatientAsync(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return patient;
    }

    public async Task<Patient> UpdatePatientAsync(Patient patient)
    {
        _context.Entry(patient).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return patient;
    }

    public async Task<bool> DeletePatientAsync(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null) return false;
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<InsuranceCompany>> GetPatientInsurancesAsync(int patientId)
    {
        var patient = await _context.Patients
            .Include(p => p.PatientInsurances)
            .ThenInclude(pi => pi.Insurance)
            .FirstOrDefaultAsync(p => p.PatientId == patientId);
        return patient?.PatientInsurances.Select(pi => pi.Insurance) ?? new List<InsuranceCompany>();
    }

    public async Task AddInsuranceToPatientAsync(int patientId, int insuranceId)
    {
        var exists = await _context.PatientInsurances
            .AnyAsync(pi => pi.PatientId == patientId && pi.InsuranceId == insuranceId);
        if (!exists)
        {
            _context.PatientInsurances.Add(new PatientInsurance
            {
                PatientId = patientId,
                InsuranceId = insuranceId
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveInsuranceFromPatientAsync(int patientId, int insuranceId)
    {
        var link = await _context.PatientInsurances
            .FirstOrDefaultAsync(pi => pi.PatientId == patientId && pi.InsuranceId == insuranceId);
        if (link != null)
        {
            _context.PatientInsurances.Remove(link);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Patient>> GetPensionPatientsAsync()
    {
        // Используем представление get_pension_patients
        return await _context.Patients
            .FromSqlRaw("SELECT * FROM get_pension_patients")
            .ToListAsync();
    }

    public async Task<IEnumerable<PatientWithAnalyzesDto>> GetPatientsWithAnalyzesAsync()
    {
        var results = await _context.Patients
            .Join(_context.Appointments, p => p.PatientId, a => a.PatientId, (p, a) => new { p, a })
            .Join(_context.Results, pa => pa.a.AppointmentId, r => r.ReferralId, (pa, r) => new { pa.p, pa.a, r })
            .GroupBy(x => x.p.PatientId)
            .Select(g => new PatientWithAnalyzesDto
            {
                PatientId = g.Key,
                FullName = g.First().p.LastName + " " + g.First().p.FirstName + " " + g.First().p.MiddleName,
                AnalyzesCount = g.Count(),
                Frequency = g.Count() < 5 ? "редкий" : "частый"
            })
            .OrderByDescending(dto => dto.AnalyzesCount)
            .ToListAsync();
        return results;
    }
}

