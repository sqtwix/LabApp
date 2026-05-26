using LabApp.Domain.Entities;

namespace LabApp.Application.Interfaces;

public interface IServiceService
{
    Task<IEnumerable<Service>> GetAllServicesAsync();
    Task<Service?> GetServiceByIdAsync(int staffId, int researchId, int appointmentId);
    Task<Service> CreateServiceAsync(Service service);
    Task<Service> UpdateServiceAsync(Service service);
    Task<bool> DeleteServiceAsync(int staffId, int researchId, int appointmentId);
}