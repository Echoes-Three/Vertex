using System.Configuration;
using System.Data;
using System.Security.Authentication.ExtendedProtection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Vertex.Models.DataServices.DataHandling;
using Vertex.Services;
using Vertex.ViewModels;

namespace Vertex;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; }
    protected override void OnStartup(StartupEventArgs e)
    {
        var dataService = new DataService();
        
        var services = new ServiceCollection();
        
        services.AddSingleton<ActivitiesHandler>();
        services.AddSingleton<RemindersHandler>();
        services.AddSingleton<SleepHandler>();
        services.AddSingleton<ConsistencyHandler>();
        services.AddSingleton<BreaksHandler>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<ActivitiesViewModel>();
        services.AddSingleton<RemindersViewModel>();
        services.AddSingleton<SleepViewModel>();
        services.AddSingleton<ConsistencyViewModel>();
        services.AddSingleton<BreaksViewModel>();

        ServiceProvider = services.BuildServiceProvider();

        ServiceProvider.GetRequiredService<ActivitiesHandler>().Load();
        ServiceProvider.GetRequiredService<RemindersHandler>().Load();
        ServiceProvider.GetRequiredService<SleepHandler>().Load();
        ServiceProvider.GetRequiredService<ConsistencyHandler>().Load();
        ServiceProvider.GetRequiredService<BreaksHandler>().Load();
        
    }
}