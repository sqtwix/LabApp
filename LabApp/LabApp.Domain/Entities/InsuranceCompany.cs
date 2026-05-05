using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class InsuranceCompany
{
    public int InsuranceId { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
