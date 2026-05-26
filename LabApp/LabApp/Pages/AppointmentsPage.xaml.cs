using System.Windows;
using System.Windows.Controls;
using LabApp.Domain.Entities;
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
            var appointment = e.Row.Item as Appointment;
            if (appointment != null)
            {
                await _viewModel.SaveAppointmentAsync(appointment);
            }
        }
    }

    private async void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var combo = sender as ComboBox;
        var appointment = combo?.DataContext as Appointment;
        if (appointment == null) return;

        string newStatus = combo.SelectedItem as string;
        if (string.IsNullOrEmpty(newStatus)) return;

        bool isMissed = (newStatus == "Не пришёл");
        await _viewModel.UpdateStatusAsync(appointment, isMissed);
    }

    private async void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_viewModel.SelectedAppointment != null)
        {
            await _viewModel.LoadResultsForAppointmentCommand.ExecuteAsync(null);
        }
    }
}