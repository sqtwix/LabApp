using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class Result
{
    public int ReferralId { get; set; }

    public int ResearchId { get; set; }

    public string? Description { get; set; }

    public DateOnly? CompletionDate { get; set; }

    public int? CarrierTypeId { get; set; }

    public virtual ResultCarrierType? CarrierType { get; set; }

    public virtual Appointment Referral { get; set; } = null!;

    public virtual Research Research { get; set; } = null!;
}
