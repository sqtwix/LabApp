using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class Biomaterial
{
    public int BiomaterialId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Indicator> Indicators { get; set; } = new List<Indicator>();
}
