using LabApp.Application.Interfaces;
using LabApp.Infrastructure.Services;
using LabApp.Infrustructure;
using LabApp.Infrustructure.Services;
using LabApp.WPF;
using LabApp.WPF.Pages;
using LabApp.WPF.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;


namespace LabApp;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private IServiceProvider _serviceProvider;
    private IConfiguration _configuration;

    protected override void OnStartup(StartupEventArgs e)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        _configuration = builder.Build();

        var services = new ServiceCollection();

        RegistrateDbContext(services);

        RegistrateServices(services);

        RegistrateViewModels(services);

        _serviceProvider = services.BuildServiceProvider();

        var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
        loginWindow.Show();

        base.OnStartup(e);
    }

    // Registration db context
    private void RegistrateDbContext(IServiceCollection services)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<LabContext>(options =>
            options.UseNpgsql(connectionString));
    }

    // Services Registration for DI
    private void RegistrateServices(IServiceCollection services) 
    {
        services.AddTransient<IPatientService, PatientService>();
        services.AddTransient<IAppointmentService, AppointmentService>();
        services.AddTransient<IResearchService, ResearchService>();
        services.AddTransient<IStaffService, StaffService>();
        services.AddTransient<IEquipmentService, EquipmentService>();
        services.AddTransient<IReportingService, ReportingService>();
        services.AddTransient<IAdminService, AdminService>();
    }

    // ViewModel Registration for MVVM
    private void RegistrateViewModels(IServiceCollection services)
    {
        services.AddTransient<MainWindow>();
        services.AddTransient<LoginWindow>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MainWindow>();
        services.AddTransient<PatientPageViewModel>();
        services.AddTransient<PatientPage>();
        services.AddTransient<StaffPageViewModel>();
        services.AddTransient<StaffPage>();
        services.AddTransient<AppointmentsPageViewModel>();
        services.AddTransient<AppointmentsPage>();
        services.AddTransient<ResearchesPageViewModel>();
        services.AddTransient<ResearchesPage>();
        services.AddTransient<ReportsPageViewModel>();
        services.AddTransient<ReportsPage>();
    }
}


