using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class Service
{
    public int StaffId { get; set; }

    public int ResearchId { get; set; }

    public int? PaymentTypeId { get; set; }

    public DateOnly? ServiceDate { get; set; }

    public int AppointmentId { get; set; }

    public int? DepartmentId { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Department? Department { get; set; }

    public virtual PaymentType? PaymentType { get; set; }

    public virtual Research Research { get; set; } = null!;

    public virtual ICollection<ServiceIndicator> ServiceIndicators { get; set; } = new List<ServiceIndicator>();

    public virtual Staff Staff { get; set; } = null!;
}
