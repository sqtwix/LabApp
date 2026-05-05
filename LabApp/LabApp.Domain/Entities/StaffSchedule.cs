using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class StaffSchedule
{
    public int ScheduleId { get; set; }

    public string? DayOfWeek { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int? StaffId { get; set; }

    public int? RoomId { get; set; }

    public virtual Room? Room { get; set; }

    public virtual Staff? Staff { get; set; }
}
