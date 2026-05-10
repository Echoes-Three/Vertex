using Vertex.MVVM;
using Vertex.Services;
using System.Windows.Media;
using Vertex.Models;
using Vertex.Models.DataServices.DataHandling;
using Vertex.Models.Entities.Entry;
using Vertex.Models.EnumDefinitions;
using Vertex.Models.UserDataHandling;
using Vertex.ViewModels.Reminders;

namespace Vertex.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ActivitiesHandler ActivitiesData { get; set; }
    private RemindersHandler RemindersData { get; set; }
    private SleepHandler SleepData { get; set; }
    private BreaksHandler BreaksData { get; set; }
    private ConsistencyHandler ConsistencyData { get; set; }

    public ActivitiesViewModel ActivitiesVM { get; }
    public RemindersViewModel RemindersVM { get; }
    public SleepViewModel SleepVM { get; }
    public BreaksViewModel BreaksVM { get; }
    public ConsistencyViewModel ConsistencyVM { get; }

    public MainWindowViewModel(
        ActivitiesHandler activitiesData,
        RemindersHandler remindersData,
        SleepHandler sleepData,
        BreaksHandler breaksData,
        ConsistencyHandler consistencyData,
        ActivitiesViewModel activitiesViewModel,
        RemindersViewModel remindersViewModel,
        SleepViewModel sleepViewModel,
        BreaksViewModel breaksViewModel,
        ConsistencyViewModel consistencyViewModel)
    {
        ActivitiesData = activitiesData;
        RemindersData = remindersData;
        SleepData = sleepData;
        BreaksData = breaksData;
        ConsistencyData = consistencyData;

        ActivitiesVM = activitiesViewModel;
        RemindersVM = remindersViewModel;
        SleepVM = sleepViewModel;
        BreaksVM = breaksViewModel;
        ConsistencyVM = consistencyViewModel;
    }
}