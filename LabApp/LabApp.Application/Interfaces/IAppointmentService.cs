using LabApp.Domain.Entities;
using LabApp.Application.Dtos;

namespace LabApp.Application.Interfaces;

public interface IAppointmentService
{
    // Направления (appointments)
    Task<Appointment> GetAppointmentByIdAsync(int id);
    Task<IEnumerable<Appointment>> GetAppointmentsByStaffAsync(int? staffId, DateTime? constraintDate, int? patientId);
    Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto dto);
    Task<Appointment> UpdateAppointmentAsync(Appointment appointment);
    Task<bool> DeleteAppointmentAsync(int id);
    Task<(string Message, Appointment Updated)> UpdateAppointmentStatusAsync(int appointmentId, bool isMissed);

    // Услуги (services) – назначенные исследования внутри направления
    Task<IEnumerable<Service>> GetServicesByAppointmentAsync(int appointmentId);
    Task<Service> AddServiceToAppointmentAsync(int appointmentId, int staffId, int researchId, int paymentTypeId, DateTime serviceDate, int departmentId);
    Task<bool> RemoveServiceFromAppointmentAsync(int appointmentId, int researchId, int staffId);

    // Оплата: тип оплаты, общая стоимость направления
    Task<int?> GetPaymentTypeForServiceAsync(int appointmentId, int researchId, int staffId);
    Task<bool> UpdateServicePaymentTypeAsync(int appointmentId, int researchId, int staffId, int newPaymentTypeId);
    Task<decimal> GetTotalCostForAppointmentAsync(int appointmentId);
}

