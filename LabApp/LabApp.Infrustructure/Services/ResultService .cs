using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using LabApp.Infrastructure.Helpers;
using LabApp.Infrustructure;
using Microsoft.EntityFrameworkCore;

namespace LabApp.Infrastructure.Services;

public class ResultService : IResultService
{
    private readonly LabContext _context;

    public ResultService(LabContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Result>> GetAllResultsAsync()
    {
        var results = await _context.Results
            // Подгружаем связанные сущности, чтобы красиво отображать их в таблице в режиме чтения
            .Include(r => r.CarrierType)
            .Include(r => r.Research)
            .ToListAsync();

        foreach (var r in results)
            r.Description = EncryptionHelper.Decrypt(r.Description);

        return results;
    }

    // НОВЫЕ МЕТОДЫ ДЛЯ COMBOBOX (Обязательно добавьте их в интерфейс IResultService!)
    public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync() => await _context.Appointments.ToListAsync();
    public async Task<IEnumerable<Research>> GetAllResearchesAsync() => await _context.Researches.ToListAsync();
    public async Task<IEnumerable<ResultCarrierType>> GetAllCarrierTypesAsync() => await _context.ResultCarrierTypes.ToListAsync();

    public async Task<Result?> GetResultByIdAsync(int referralId, int researchId)
    {
        return await _context.Results
            .Include(r => r.CarrierType)
            .FirstOrDefaultAsync(r => r.ReferralId == referralId && r.ResearchId == researchId);
    }

    public async Task<Result> CreateResultAsync(Result result)
    {
        var plainDesc = result.Description;
        try
        {
            result.Description = EncryptionHelper.Encrypt(plainDesc ?? "");
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
        }
        finally
        {
            result.Description = plainDesc;
        }
        return result;
    }

    public async Task<Result> UpdateResultAsync(Result result)
    {
        var existing = await _context.Results
            .FirstOrDefaultAsync(r => r.ReferralId == result.ReferralId && r.ResearchId == result.ResearchId);
        if (existing == null) return null;

        var plainDesc = result.Description;
        try
        {
            existing.CompletionDate = result.CompletionDate;
            existing.CarrierTypeId = result.CarrierTypeId;
            existing.Description = EncryptionHelper.Encrypt(plainDesc ?? "");
            await _context.SaveChangesAsync();
        }
        finally
        {
            result.Description = plainDesc;
        }
        return result;
    }

    public async Task<bool> DeleteResultAsync(int referralId, int researchId)
    {
        var result = await _context.Results
            .FirstOrDefaultAsync(r => r.ReferralId == referralId && r.ResearchId == researchId);
        if (result == null) return false;

        _context.Results.Remove(result);
        await _context.SaveChangesAsync();
        return true;
    }
}