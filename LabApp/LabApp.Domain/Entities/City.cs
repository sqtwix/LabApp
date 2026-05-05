using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class City
{
    public int CityId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
