using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class Indicator
{
    public int IndicatorId { get; set; }

    public string? Name { get; set; }

    public int? BiomaterialId { get; set; }

    public decimal? Cost { get; set; }

    public string? ExecutionTime { get; set; }

    public virtual Biomaterial? Biomaterial { get; set; }

    public virtual ICollection<ServiceIndicator> ServiceIndicators { get; set; } = new List<ServiceIndicator>();
}
