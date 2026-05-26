using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using LabApp.Infrustructure;
using Microsoft.EntityFrameworkCore;

namespace LabApp.Infrastructure.Services;

public class ServiceService : IServiceService
{
    private readonly LabContext _context;

    public ServiceService(LabContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Service>> GetAllServicesAsync()
    {
        return await _context.Services
            .Include(s => s.Staff)
            .Include(s => s.Research)
            .Include(s => s.PaymentType)
            .Include(s => s.Department)
            .Include(s => s.Appointment)
            .ToListAsync();
    }

    public async Task<Service?> GetServiceByIdAsync(int staffId, int researchId, int appointmentId)
    {
        return await _context.Services
            .Include(s => s.Staff)
            .Include(s => s.Research)
            .Include(s => s.PaymentType)
            .Include(s => s.Department)
            .Include(s => s.Appointment)
            .FirstOrDefaultAsync(s => s.StaffId == staffId && s.ResearchId == researchId && s.AppointmentId == appointmentId);
    }

    public async Task<Service> CreateServiceAsync(Service service)
    {
        _context.Services.Add(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task<Service> UpdateServiceAsync(Service service)
    {
        _context.Entry(service).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task<bool> DeleteServiceAsync(int staffId, int researchId, int appointmentId)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.StaffId == staffId && s.ResearchId == researchId && s.AppointmentId == appointmentId);
        if (service == null) return false;
        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
        return true;
    }
}