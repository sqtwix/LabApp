using System.Windows.Input;

namespace LabApp.WPF.ViewModels
{
    public class MenuItemViewModel
    {
        public string Header { get; set; } = string.Empty;
        public ICommand? Command { get; set; }
        public bool IsVisible { get; set; } = true;
    }
}