using System;
using System.Collections.Generic;

namespace LabApp.Domain.Entities;

public partial class GetDeprtmentsWithStaffCount
{
    public int? IdОтделения { get; set; }

    public string? НазваниеОтделения { get; set; }

    public long? КолВоСотурдниковВОтделении { get; set; }
}
