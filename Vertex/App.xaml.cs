using System.Configuration;
using System.Data;
using System.Windows;
using Vertex.Models.DataServices.DataHandling;
using Vertex.Models.StressUnit;
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
        var breakData = new BreaksHandler();
        var dailyData = new ActivitiesHandler();
        var reminderData = new RemindersHandler();
        var weeklyData = new ConsistencyHandler();

        var scores = new ScoreData();
        scores.GetScores(dailyData);
    }
}