using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class Research
{
    public int ResearchId { get; set; }

    public int ResearchTypeId { get; set; }

    public string? Name { get; set; }

    public decimal? Cost { get; set; }

    public int? ReagentId { get; set; }

    public virtual Reagent? Reagent { get; set; }

    public virtual ResearchType ResearchType { get; set; } = null!;

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();

    public virtual ICollection<Reagent> Reagents { get; set; } = new List<Reagent>();
}
