using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class ServiceIndicator
{
    public int ServiceId { get; set; }

    public int IndicatorId { get; set; }

    public int? StaffId { get; set; }

    public int? ResearchId { get; set; }

    public int? AppointmentId { get; set; }

    public virtual Indicator Indicator { get; set; } = null!;

    public virtual Service? Service { get; set; }
}
