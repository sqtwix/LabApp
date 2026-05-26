using System.Windows.Controls;
using LabApp.WPF.ViewModels;

namespace LabApp.WPF.Pages;

public partial class PatientPage : Page
{
    private PatientPageViewModel _viewModel;
    private bool _isSaving = false; // Защита от двойного вызова

    public PatientPage(PatientPageViewModel viewModel)
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

            var patient = e.Row.Item as Domain.Entities.Patient;
            if (patient != null)
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    _isSaving = true;
                    try
                    {
                        await _viewModel.SavePatientAsync(patient);
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