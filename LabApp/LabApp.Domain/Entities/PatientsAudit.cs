using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class PatientsAudit
{
    public int AuditId { get; set; }

    public string? OperationType { get; set; }

    public string? ChangedBy { get; set; }

    public DateTime? ChangedAt { get; set; }

    public int? PatientId { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Gender { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int? CityId { get; set; }

    public string? Passport { get; set; }
}
