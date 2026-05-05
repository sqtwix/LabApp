using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class GetPensionPatient
{
    public int? IdПациента { get; set; }

    public string? Фио { get; set; }

    public string? Пол { get; set; }

    public decimal? Возраст { get; set; }
}
