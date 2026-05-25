namespace LabApp.Application.Dtos;
public class CreateAppointmentDto
{
    public int? StaffId { get; set; }
    public int? PatientId { get; set; }
    public TimeSpan? AppointmentTime { get; set; }
    public DateTime? AppointmentDate { get; set; }
    public string? Status { get; set; } = "Запланирована";
    
}
