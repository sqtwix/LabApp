using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class ResearchType
{
    public int ResearchTypeId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Research> Researches { get; set; } = new List<Research>();
}
