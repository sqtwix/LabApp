using System.Windows.Controls;
using LabApp.WPF.ViewModels;

namespace LabApp.WPF.Pages;

public partial class AppointmentsPage : Page
{
    private AppointmentsPageViewModel _viewModel;

    public AppointmentsPage(AppointmentsPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
    }

    private async void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            var appointment = e.Row.Item as Domain.Entities.Appointment;
            if (appointment != null)
            {
                await _viewModel.SaveAppointmentAsync(appointment);
            }
        }
    }
}