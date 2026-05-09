namespace LabApp.Domain.Entities
{
    public class StaffDepartment
    {
        public int StaffId { get; set; }
        public int DepartmentId { get; set; }
        public virtual Staff Staff { get; set; } = null!;
        public virtual Department Department { get; set; } = null!;
    }
}
