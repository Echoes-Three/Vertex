using System.Configuration;
using System.Data;
using System.Windows;
using Vertex.Models.UserData.DataHandling;

namespace Vertex;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; private set; }
    protected override void OnStartup(StartupEventArgs e)
    {
        var breakData = new BreakData();
        var dailyData = new DailyData();
        var reminderData = new ReminderData();
        var weeklyData = new WeeklyData();
    }
}