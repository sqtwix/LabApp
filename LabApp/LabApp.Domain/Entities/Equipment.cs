using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class Equipment
{
    public int EquipmentId { get; set; }

    public string? Name { get; set; }

    public int? RoomId { get; set; }

    public DateOnly? ExplotationDate { get; set; }

    public int? ServiceLife { get; set; }

    public virtual Room? Room { get; set; }

    public virtual ICollection<Research> Researches { get; set; } = new List<Research>();
}
