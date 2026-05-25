namespace LabApp.Application.Dtos
{
    public class DepartmentStaffCountDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int StaffCount { get; set; }
    }
}
