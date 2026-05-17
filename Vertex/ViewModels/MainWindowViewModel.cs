using Vertex.MVVM;
using System.Windows.Media;
using Vertex.Data.Handlers;
using Vertex.Models;
using Vertex.Models.Entities.Entry;
using Vertex.Models.EnumDefinitions;
using Vertex.ViewModels.Activities;
using Vertex.ViewModels.DonutGraph;
using Vertex.ViewModels.Reminders;

namespace Vertex.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ActivitiesHandler ActivitiesData { get; set; }
    private RemindersHandler RemindersData { get; set; }
    private SleepHandler SleepData { get; set; }
    private ConsistencyHandler ConsistencyData { get; set; }
    
    public ActivitiesViewModel ActivitiesVM { get; }
    public RemindersViewModel RemindersVM { get; }
    public SleepViewModel SleepVM { get; }
    public ConsistencyViewModel ConsistencyVM { get; }
    public  DonutGraphViewModel DonutGraphVM { get; }

    public MainWindowViewModel(
        ActivitiesHandler activitiesData,
        RemindersHandler remindersData,
        SleepHandler sleepData,
        ConsistencyHandler consistencyData,
        
        ActivitiesViewModel activitiesViewModel,
        RemindersViewModel remindersViewModel,
        SleepViewModel sleepViewModel,
        ConsistencyViewModel consistencyViewModel,
        DonutGraphViewModel  donutGraphViewModel)
    {
        ActivitiesData = activitiesData;
        RemindersData = remindersData;
        SleepData = sleepData;
        ConsistencyData = consistencyData;
        
        ActivitiesVM = activitiesViewModel;
        RemindersVM = remindersViewModel;
        SleepVM = sleepViewModel;
        ConsistencyVM = consistencyViewModel;
        DonutGraphVM = donutGraphViewModel;
    }
}