using System.Windows.Controls;
using LabApp.WPF.ViewModels;

namespace LabApp.WPF.Pages;

public partial class ReportsPage : Page
{
    public ReportsPage(ReportsPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}