using System.Windows.Controls;
using LabApp.WPF.ViewModels;

namespace LabApp.WPF.Pages;

public partial class PatientPage : Page
{
    private PatientPageViewModel _viewModel;

    public PatientPage(PatientPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
    }

    private async void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            var patient = e.Row.Item as Domain.Entities.Patient;
            if (patient != null)
            {
                await _viewModel.SavePatientAsync(patient);
            }
        }
    }
}