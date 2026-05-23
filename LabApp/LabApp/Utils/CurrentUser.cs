namespace LabApp.WPF.Utils;

public class CurrentUser
{
    public static int StaffId { get; set; }
    public static string? Role {  get; set; } = string.Empty;
    public static bool IsAuthenticated => StaffId > 0 && !string.IsNullOrEmpty(Role);
}

