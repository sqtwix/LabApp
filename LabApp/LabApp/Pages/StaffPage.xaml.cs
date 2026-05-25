using System.Windows.Controls;
using LabApp.WPF.ViewModels;

namespace LabApp.WPF.Pages;

public partial class StaffPage : Page
{
    private StaffPageViewModel _viewModel;

    public StaffPage(StaffPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
    }

    private async void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            var staff = e.Row.Item as Domain.Entities.Staff;
            if (staff != null)
            {
                await _viewModel.SaveStaffAsync(staff);
            }
        }
    }
}