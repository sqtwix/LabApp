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
        var results = await _context.Results.ToListAsync();
        foreach (var r in results)
            r.Description = EncryptionHelper.Decrypt(r.Description);
        return results;
    }

    public async Task<Result?> GetResultByIdAsync(int referralId, int researchId)
    {
        return await _context.Results
            .Include(r => r.CarrierType)
            .FirstOrDefaultAsync(r => r.ReferralId == referralId && r.ResearchId == researchId);
    }

    public async Task<Result> CreateResultAsync(Result result)
    {
        result.Description = EncryptionHelper.Encrypt(result.Description);
        _context.Results.Add(result);
        await _context.SaveChangesAsync();
        result.Description = EncryptionHelper.Decrypt(result.Description);
        return result;
    }

    public async Task<Result> UpdateResultAsync(Result result)
    {
        var existing = await _context.Results
            .FirstOrDefaultAsync(r => r.ReferralId == result.ReferralId && r.ResearchId == result.ResearchId);
        if (existing == null) return null;

        existing.Description = EncryptionHelper.Encrypt(result.Description);
        existing.CompletionDate = result.CompletionDate;
        existing.CarrierTypeId = result.CarrierTypeId;

        await _context.SaveChangesAsync();
        result.Description = EncryptionHelper.Decrypt(existing.Description);
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