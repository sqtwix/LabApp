namespace LabApp.Domain.Entities;

public partial class Department
{
    public int DepartmentId { get; set; }
    public string? Name { get; set; }
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    public virtual ICollection<StaffDepartment> StaffDepartments { get; set; } = new List<StaffDepartment>();
}