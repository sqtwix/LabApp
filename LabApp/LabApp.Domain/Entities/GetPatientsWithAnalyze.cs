using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class GetPatientsWithAnalyze
{
    public int? IdПациента { get; set; }

    public string? ФиоПациента { get; set; }

    public long? КолВоАнализов { get; set; }

    public string? ЧастотаОбращений { get; set; }
}
