using System.Windows.Controls;
using LabApp.Domain.Entities;
using LabApp.WPF.ViewModels;

namespace LabApp.WPF.Pages;

public partial class ResultsPage : Page
{
    private ResultsPageViewModel _viewModel;

    public ResultsPage(ResultsPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
    }

    private async void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            var result = e.Row.Item as Result;
            if (result != null)
            {
                await _viewModel.SaveResultAsync(result);
            }
        }
    }
}