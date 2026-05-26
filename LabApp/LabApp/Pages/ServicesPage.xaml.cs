using System.Windows.Controls;
using LabApp.Domain.Entities;
using LabApp.WPF.ViewModels;

namespace LabApp.WPF.Pages;

public partial class ServicesPage : Page
{
    private ServicesPageViewModel _viewModel;

    public ServicesPage(ServicesPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
    }

    private async void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            var service = e.Row.Item as Service;
            if (service != null)
            {
                await _viewModel.SaveServiceAsync(service);
            }
        }
    }
}