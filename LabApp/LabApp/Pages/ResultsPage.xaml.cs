using System.Windows.Controls;
using LabApp.Domain.Entities;
using LabApp.WPF.ViewModels;

namespace LabApp.WPF.Pages;

public partial class ResultsPage : Page
{
    private ResultsPageViewModel _viewModel;
    private bool _isSaving = false;

    public ResultsPage(ResultsPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
    }

    private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            if (_isSaving) return;

            var result = e.Row.Item as Result;
            if (result != null)
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    _isSaving = true;
                    try
                    {
                        await _viewModel.SaveResultAsync(result);
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