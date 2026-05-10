namespace LabApp.Domain.Entities;

public partial class Specialization
{
    public int SpecializationId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<StaffSpecialization> StaffSpecializations { get; set; } = new List<StaffSpecialization>();
}
