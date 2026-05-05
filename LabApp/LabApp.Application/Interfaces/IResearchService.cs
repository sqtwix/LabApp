using LabApp.Domain.Entities;

namespace LabApp.Application.Interfaces;

public interface IResearchService
{
    // Исследования
    Task<Research> GetResearchByIdAsync(int id);
    Task<IEnumerable<Research>> GetAllResearchesAsync();
    Task<Research> CreateResearchAsync(Research research);
    Task<Research> UpdateResearchAsync(Research research);
    Task<bool> DeleteResearchAsync(int id);

    // Результаты
    Task<Result> GetResultByReferralAndResearchAsync(int referralId, int researchId);
    Task<Result> AddOrUpdateResultAsync(int referralId, int researchId, string description, DateTime completionDate, int? carrierTypeId);

    // Справочные таблицы
    Task<IEnumerable<Indicator>> GetAllIndicatorsAsync();
    Task<IEnumerable<Biomaterial>> GetAllBiomaterialsAsync();
    Task<IEnumerable<ResearchType>> GetAllResearchTypesAsync();
    Task<IEnumerable<ResultCarrierType>> GetAllCarrierTypesAsync();
}
