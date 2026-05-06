using Humanizer;
using LabApp.Application.Dtos;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabApp.Infrustructure.Services;
public class AppointmentService : IAppointmentService
{
    private readonly LabContext _context;

    public AppointmentService(LabContext context)
    {
        _context = context;
    }

    public async Task<Appointment> GetAppointmentByIdAsync(int id)
    {
        return await _context.Appointments
            .Include(a => a.Staff)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.AppointmentId == id);
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByStaffAsync(int? staffId, DateTime? constraintDate, int? patientId)
    {
        // Используем функцию get_appointments
        return await _context.Appointments
            .FromSqlRaw("SELECT * FROM get_appointments({0}, {1}, {2})", staffId, constraintDate, patientId)
            .ToListAsync();
    }

    public async Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto dto)
    {
        var appointment = new Appointment
        {
            StaffId = dto.StaffId,
            PatientId = dto.PatientId,
            AppointmentTime = TimeOnly.FromTimeSpan(dto.AppointmentTime),
            AppointmentDate = DateOnly.FromDateTime(dto.AppointmentDate),
            Status = dto.Status ?? "Запланирована"
        };
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task<Appointment> UpdateAppointmentAsync(Appointment appointment)
    {
        _context.Entry(appointment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task<bool> DeleteAppointmentAsync(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return false;
        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(string Message, Appointment Updated)> UpdateAppointmentStatusAsync(int appointmentId, bool isMissed)
    {
        // Вызов хранимой функции update_appointment_status
        var result = await _context.Appointments
            .FromSqlRaw("SELECT * FROM update_appointment_status({0}, {1})", appointmentId, isMissed)
            .ToListAsync();
        var updated = result.FirstOrDefault();
        if (updated == null)
            return ("Запись не найдена", null);
        return ("Успешно обновлено", updated);
    }

    public async Task<IEnumerable<Service>> GetServicesByAppointmentAsync(int appointmentId)
    {
        return await _context.Services
            .Where(s => s.AppointmentId == appointmentId)
            .Include(s => s.Research)
            .Include(s => s.PaymentType)
            .ToListAsync();
    }

    public async Task<Service> AddServiceToAppointmentAsync(int appointmentId, int staffId, int researchId, int paymentTypeId, DateTime serviceDate, int departmentId)
    {
        var service = new Service
        {
            AppointmentId = appointmentId,
            StaffId = staffId,
            ResearchId = researchId,
            PaymentTypeId = paymentTypeId,
            ServiceDate = DateOnly.FromDateTime(serviceDate),
            DepartmentId = departmentId
        };
        _context.Services.Add(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task<bool> RemoveServiceFromAppointmentAsync(int appointmentId, int researchId, int staffId)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.AppointmentId == appointmentId && s.ResearchId == researchId && s.StaffId == staffId);
        if (service == null) return false;
        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int?> GetPaymentTypeForServiceAsync(int appointmentId, int researchId, int staffId)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.AppointmentId == appointmentId && s.ResearchId == researchId && s.StaffId == staffId);
        return service?.PaymentTypeId;
    }

    public async Task<bool> UpdateServicePaymentTypeAsync(int appointmentId, int researchId, int staffId, int newPaymentTypeId)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.AppointmentId == appointmentId && s.ResearchId == researchId && s.StaffId == staffId);
        if (service == null) return false;
        service.PaymentTypeId = newPaymentTypeId;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalCostForAppointmentAsync(int appointmentId)
    {
        var total = await _context.Services
            .Where(s => s.AppointmentId == appointmentId)
            .Join(_context.Researches, s => s.ResearchId, r => r.ResearchId, (s, r) => r.Cost ?? 0)
            .SumAsync();
        return total;
    }
}

