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

    private bool _isSaving = false;

    private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            var staff = e.Row.Item as Domain.Entities.Staff;
            if (staff != null)
            {
                if (_isSaving) return;

                Dispatcher.InvokeAsync(async () =>
                {
                    _isSaving = true;
                    try
                    {
                        await _viewModel.SaveStaffAsync(staff);
                    }
                    finally
                    {
                        _isSaving = false;
                    }
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
        }
    }
}