using LabApp.WPF.ViewModels;
using System.Windows;

namespace LabApp.WPF;
    
public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
