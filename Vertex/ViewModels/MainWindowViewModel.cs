using Vertex.MVVM;
using Vertex.Services;
using System.Windows.Media;
using Vertex.Models;
using Vertex.Models.EnumDefinitions;
using Vertex.Models.UserData.DataHandling;
using Vertex.Models.UserData.Entry;
using Vertex.Models.UserDataHandling;

namespace Vertex.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        var dailyActivityViewModel = new DailyActivitiesViewModel();
        var breaksViewModel = new BreaksViewModel();
        var remindersViewModel = new RemindersViewModel();
        var weeklySnapshotViewModel = new WeeklySnapshotViewModel();
        var dataService = new DataService();
        
    }
    
}