using System.Windows;
using System.Windows.Controls;
using LabApp.Domain.Entities;
using LabApp.WPF.ViewModels;

namespace LabApp.WPF.Pages;

public partial class AppointmentsPage : Page
{
    private AppointmentsPageViewModel _viewModel;
    private bool _isSaving = false;

    public AppointmentsPage(AppointmentsPageViewModel viewModel)
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

            var appointment = e.Row.Item as Appointment;
            if (appointment != null)
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    _isSaving = true;
                    try
                    {
                        await _viewModel.SaveAppointmentAsync(appointment);
                    }
                    finally
                    {
                        _isSaving = false;
                    }
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
        }
    }

    private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var combo = sender as ComboBox;

        // ВАЖНО: Защита от авто-срабатывания WPF при загрузке данных в DataGrid
        if (combo == null || !combo.IsDropDownOpen) return;

        // Защита от параллельного сохранения строки
        if (_isSaving || _viewModel.IsBusy) return;

        var appointment = combo.DataContext as Appointment;
        if (appointment == null) return;

        string newStatus = combo.SelectedItem as string;
        if (string.IsNullOrEmpty(newStatus)) return;

        bool isMissed = (newStatus == "Не пришёл");

        // Отправляем в очередь, чтобы не вешать UI-поток
        Dispatcher.InvokeAsync(async () =>
        {
            await _viewModel.UpdateStatusAsync(appointment, isMissed);
        });
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Не пытаемся грузить результаты, если строка сейчас сохраняется!
        if (_viewModel.SelectedAppointment != null && !_isSaving && !_viewModel.IsBusy)
        {
            // Безопасный вызов команды
            _viewModel.LoadResultsForAppointmentCommand.Execute(null);
        }
    }
}