namespace LabApp.Domain.Entities
{
    public class StaffSpecialization
    {
        public int StaffId { get; set; }
        public int SpecializationId { get; set; }
        public virtual Staff Staff { get; set; } = null!;
        public virtual Specialization Specialization { get; set; } = null!;
    }
}
