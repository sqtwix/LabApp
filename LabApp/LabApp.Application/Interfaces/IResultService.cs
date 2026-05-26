using LabApp.Domain.Entities;

namespace LabApp.Application.Interfaces;

public interface IResultService
{
    // Чтение
    Task<IEnumerable<Result>> GetAllResultsAsync();
    Task<Result?> GetResultByIdAsync(int referralId, int researchId);

    // Создание
    Task<Result> CreateResultAsync(Result result);

    // Обновление
    Task<Result> UpdateResultAsync(Result result);

    // Удаление
    Task<bool> DeleteResultAsync(int referralId, int researchId);
}