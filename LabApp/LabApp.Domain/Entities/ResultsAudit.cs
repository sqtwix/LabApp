using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class ResultsAudit
{
    public int AuditId { get; set; }

    public int? ReferralId { get; set; }

    public int? ResearchId { get; set; }

    public string? OperationType { get; set; }

    public string? ChangedBy { get; set; }

    public DateTime? ChangedAt { get; set; }

    public string? OldDescription { get; set; }

    public DateOnly? OldCompletionDate { get; set; }

    public int? OldCarrierTypeId { get; set; }
}
