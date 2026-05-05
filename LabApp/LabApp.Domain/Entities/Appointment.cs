using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int? StaffId { get; set; }

    public int? PatientId { get; set; }

    public TimeOnly? AppointmentTime { get; set; }

    public string? Status { get; set; }

    public DateOnly? AppointmentDate { get; set; }

    public virtual Patient? Patient { get; set; }

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual Staff? Staff { get; set; }
}
