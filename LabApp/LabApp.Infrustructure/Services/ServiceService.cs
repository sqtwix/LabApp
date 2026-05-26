using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using LabApp.Infrustructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    // --- НОВЫЕ МЕТОДЫ ДЛЯ ВЫПАДАЮЩИХ СПИСКОВ ---
    public async Task<IEnumerable<Staff>> GetAllStaffsAsync() => await _context.Staffs.ToListAsync();
    public async Task<IEnumerable<Research>> GetAllResearchesAsync() => await _context.Researches.ToListAsync();
    public async Task<IEnumerable<PaymentType>> GetAllPaymentTypesAsync() => await _context.PaymentTypes.ToListAsync();
    public async Task<IEnumerable<Department>> GetAllDepartmentsAsync() => await _context.Departments.ToListAsync();
    public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync() => await _context.Appointments.ToListAsync();

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
        return await GetServiceByIdAsync(service.StaffId, service.ResearchId, service.AppointmentId);
    }

    public async Task<Service> UpdateServiceAsync(Service service)
    {
        // При составном ключе EF Core не позволяет менять сами ключи. 
        // Мы обновляем только не-ключевые поля: дату, тип оплаты и отделение.
        var existing = await _context.Services.FirstOrDefaultAsync(s =>
            s.StaffId == service.StaffId &&
            s.ResearchId == service.ResearchId &&
            s.AppointmentId == service.AppointmentId);

        if (existing == null) return null;

        existing.ServiceDate = service.ServiceDate;
        existing.PaymentTypeId = service.PaymentTypeId;
        existing.DepartmentId = service.DepartmentId;

        await _context.SaveChangesAsync();
        return existing;
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