using LabApp.Application.Dtos;
using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        return await _context.Appointments
            .FromSqlRaw("SELECT * FROM get_appointments({0}, {1}, {2})", staffId, constraintDate, patientId)
            .ToListAsync();
    }

    public async Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto dto)
    {
        // Ручная генерация ID, так как в БД нет SERIAL для этой таблицы
        var maxId = await _context.Appointments.MaxAsync(a => (int?)a.AppointmentId) ?? 0;

        var appointment = new Appointment
        {
            AppointmentId = maxId + 1,
            StaffId = dto.StaffId,
            PatientId = dto.PatientId,
            AppointmentTime = dto.AppointmentTime.HasValue
                ? TimeOnly.FromTimeSpan(dto.AppointmentTime.Value)
                : null,
            AppointmentDate = dto.AppointmentDate.HasValue
                ? DateOnly.FromDateTime(dto.AppointmentDate.Value)
                : null,
            Status = dto.Status ?? "Запланирована"
        };
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // Подгружаем навигационные свойства для красивого отображения в UI
        return await GetAppointmentByIdAsync(appointment.AppointmentId);
    }

    public async Task<Appointment> UpdateAppointmentAsync(Appointment appointment)
    {
        var existing = await _context.Appointments.FindAsync(appointment.AppointmentId);
        if (existing == null) return null;

        existing.StaffId = appointment.StaffId;
        existing.PatientId = appointment.PatientId;
        existing.AppointmentDate = appointment.AppointmentDate;
        existing.AppointmentTime = appointment.AppointmentTime;
        existing.Status = appointment.Status;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAppointmentAsync(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return false;
        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(string Message, Appointment UpdatedAppointment)> UpdateAppointmentStatusAsync(int appointmentId, bool isMissed)
    {
        string message = "Статус обновлен";

        // 1. Вызываем функцию через ADO.NET, чтобы избежать проблем с маппингом композитных типов EF Core
        var connection = _context.Database.GetDbConnection();
        using var command = connection.CreateCommand();

        // Синтаксис (updated_appointment).* распаковывает композитный тип, если бы мы захотели читать его поля,
        // но нам достаточно получить только сообщение от функции
        command.CommandText = "SELECT message FROM update_appointment_status(@p0, @p1)";

        var p0 = command.CreateParameter();
        p0.ParameterName = "@p0";
        p0.Value = appointmentId;
        command.Parameters.Add(p0);

        var p1 = command.CreateParameter();
        p1.ParameterName = "@p1";
        p1.Value = isMissed;
        command.Parameters.Add(p1);

        await _context.Database.OpenConnectionAsync();
        using (var reader = await command.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                message = reader.GetString(0);
            }
        }
        await _context.Database.CloseConnectionAsync();

        // 2. Загружаем обновленную сущность через EF Core, 
        // чтобы подтянулись навигационные свойства (Patient, Staff) для DataGrid
        var updated = await GetAppointmentByIdAsync(appointmentId);

        return (message, updated);
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Staff)
            .ToListAsync();
    }

    public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
    {
        return await _context.Patients.ToListAsync();
    }

    public async Task<IEnumerable<Staff>> GetAllStaffAsync()
    {
        return await _context.Staffs.ToListAsync();
    }

    // --- ВОССТАНОВЛЕННЫЕ МЕТОДЫ УСЛУГ ---

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