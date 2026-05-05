using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class Specialization
{
    public int SpecializationId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
