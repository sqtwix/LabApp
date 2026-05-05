using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class Reagent
{
    public int ReagentId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Research> Researches { get; set; } = new List<Research>();

    public virtual ICollection<Research> ResearchesNavigation { get; set; } = new List<Research>();
}
