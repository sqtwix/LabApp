using System.Windows.Controls;
using LabApp.WPF.ViewModels;

namespace LabApp.WPF.Pages;

public partial class ResearchesPage : Page
{
    private ResearchesPageViewModel _viewModel;

    public ResearchesPage(ResearchesPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
    }

    private async void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            var research = e.Row.Item as Domain.Entities.Research;
            if (research != null)
            {
                await _viewModel.SaveResearchAsync(research);
            }
        }
    }
}