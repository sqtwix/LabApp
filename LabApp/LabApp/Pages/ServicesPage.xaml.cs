using System.Windows.Controls;
using LabApp.Domain.Entities;
using LabApp.WPF.ViewModels;

namespace LabApp.WPF.Pages;

public partial class ServicesPage : Page
{
    private ServicesPageViewModel _viewModel;
    private bool _isSaving = false;

    public ServicesPage(ServicesPageViewModel viewModel)
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

            var service = e.Row.Item as Service;
            if (service != null)
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    _isSaving = true;
                    try
                    {
                        await _viewModel.SaveServiceAsync(service);
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