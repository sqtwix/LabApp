using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class ResultCarrierType
{
    public int CarrierTypeId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}
