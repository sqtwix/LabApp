using System.Windows.Controls;
using LabApp.Domain.Entities;
using LabApp.WPF.ViewModels;

namespace LabApp.WPF.Pages;

public partial class EquipmentPage : Page
{
    private EquipmentPageViewModel _viewModel;

    public EquipmentPage(EquipmentPageViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
    }

    private async void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            var equipment = e.Row.Item as Equipment;
            if (equipment != null)
            {
                await _viewModel.SaveEquipmentAsync(equipment);
            }
        }
    }
}