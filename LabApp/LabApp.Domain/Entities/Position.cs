using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class Position
{
    public int PositionId { get; set; }

    public string? Name { get; set; }

    public decimal? Salary { get; set; }

    public string? Duties { get; set; }

    public string? EducationLevel { get; set; }

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
