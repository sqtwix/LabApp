using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace LabApp.Infrustructure.Services;
public class ResearchService : IResearchService
{
    private readonly LabContext _context;

    public ResearchService(LabContext context)
    {
        _context = context;
    }

    public async Task<Research> GetResearchByIdAsync(int id)
    {
        return await _context.Researches
            .Include(r => r.ResearchType)
            .FirstOrDefaultAsync(r => r.ResearchId == id);
    }

    public async Task<IEnumerable<Research>> GetAllResearchesAsync()
    {
        return await _context.Researches
            .Include(r => r.ResearchType)
            .ToListAsync();
    }

    public async Task<Research> CreateResearchAsync(Research research)
    {
        _context.Researches.Add(research);
        await _context.SaveChangesAsync();
        return research;
    }

    public async Task<Research> UpdateResearchAsync(Research research)
    {
        _context.Entry(research).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return research;
    }

    public async Task<bool> DeleteResearchAsync(int id)
    {
        var research = await _context.Researches.FindAsync(id);
        if (research == null) return false;
        _context.Researches.Remove(research);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Result> GetResultByReferralAndResearchAsync(int referralId, int researchId)
    {
        return await _context.Results
            .FirstOrDefaultAsync(r => r.ReferralId == referralId && r.ResearchId == researchId);
    }

    public async Task<Result> AddOrUpdateResultAsync(int referralId, int researchId, string description, DateTime completionDate, int? carrierTypeId)
    {
        var result = await GetResultByReferralAndResearchAsync(referralId, researchId);
        if (result == null)
        {
            result = new Result
            {
                ReferralId = referralId,
                ResearchId = researchId,
                Description = description,
                CompletionDate = DateOnly.FromDateTime(completionDate),
                CarrierTypeId = carrierTypeId
            };
            _context.Results.Add(result);
        }
        else
        {
            result.Description = description;
            result.CompletionDate = DateOnly.FromDateTime(completionDate);
            result.CarrierTypeId = carrierTypeId;
            _context.Entry(result).State = EntityState.Modified;
        }
        await _context.SaveChangesAsync();
        return result;
    }

    public async Task<IEnumerable<Indicator>> GetAllIndicatorsAsync()
    {
        return await _context.Indicators
            .Include(i => i.Biomaterial)
            .ToListAsync();
    }

    public async Task<IEnumerable<Biomaterial>> GetAllBiomaterialsAsync()
    {
        return await _context.Biomaterials.ToListAsync();
    }

    public async Task<IEnumerable<ResearchType>> GetAllResearchTypesAsync()
    {
        return await _context.ResearchTypes.ToListAsync();
    }

    public async Task<IEnumerable<ResultCarrierType>> GetAllCarrierTypesAsync()
    {
        return await _context.ResultCarrierTypes.ToListAsync();
    }
}