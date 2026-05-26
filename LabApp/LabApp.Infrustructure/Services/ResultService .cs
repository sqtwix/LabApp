using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
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
        return await _context.Results
            .Include(r => r.CarrierType)
            .ToListAsync();
    }

    public async Task<Result?> GetResultByIdAsync(int referralId, int researchId)
    {
        return await _context.Results
            .Include(r => r.CarrierType)
            .FirstOrDefaultAsync(r => r.ReferralId == referralId && r.ResearchId == researchId);
    }

    public async Task<Result> CreateResultAsync(Result result)
    {
        _context.Results.Add(result);
        await _context.SaveChangesAsync();
        return result;
    }

    public async Task<Result> UpdateResultAsync(Result result)
    {
        _context.Entry(result).State = EntityState.Modified;
        await _context.SaveChangesAsync();
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