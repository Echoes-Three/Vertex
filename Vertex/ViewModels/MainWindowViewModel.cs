using Vertex.MVVM;
using Vertex.Data.Handlers;
using Vertex.ViewModels.Activities;
using Vertex.ViewModels.DonutGraph;
using Vertex.ViewModels.Reminders;

namespace Vertex.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ActivitiesHandler ActivitiesData { get; set; }
    private RemindersHandler RemindersData { get; set; }
    public ActivitiesViewModel ActivitiesVM { get; }
    public RemindersViewModel RemindersVM { get; }
    public  DonutGraphViewModel DonutGraphVM { get; }

    public MainWindowViewModel(
        ActivitiesHandler activitiesData,
        RemindersHandler remindersData,
        
        ActivitiesViewModel activitiesViewModel,
        RemindersViewModel remindersViewModel,
        DonutGraphViewModel  donutGraphViewModel)
    {
        ActivitiesData = activitiesData;
        RemindersData = remindersData;
        
        ActivitiesVM = activitiesViewModel;
        RemindersVM = remindersViewModel;
        DonutGraphVM = donutGraphViewModel;
    }
}