namespace LabApp.Domain.Entities;

public partial class Staff
{
    public int StaffId { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }
    public string? Login { get; set; }
    public string? PasswordHash { get; set; }
    public string? RoleName { get; set; }

    public string? Gender { get; set; }

    public string? Phone { get; set; }

    public string? Passport { get; set; }

    public string? Address { get; set; }

    public DateOnly? BirthDate { get; set; }

    public int? PositionId { get; set; }

    public int? CityId { get; set; }

    public string? Education { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual City? City { get; set; }

    public virtual Position? Position { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual ICollection<StaffSchedule> StaffSchedules { get; set; } = new List<StaffSchedule>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<Specialization> Specializations { get; set; } = new List<Specialization>();

}
