using Vertex.MVVM;
using Vertex.Services;
using System.Windows.Media;
using Vertex.Models;
using Vertex.Models.DataServices.DataHandling;
using Vertex.Models.Entities.Entry;
using Vertex.Models.EnumDefinitions;
using Vertex.Models.UserData.Entry;
using Vertex.Models.UserDataHandling;

namespace Vertex.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ActivitiesHandler ActivitiesData {get; set;}
    private RemindersHandler RemindersData {get; set;}
    private SleepHandler SleepData {get; set;}
    private BreaksHandler BreaksData {get; set;}
    private ConsistencyHandler ConsistencyData {get; set;}
    
    public MainWindowViewModel(
        ActivitiesHandler activitiesData,
        RemindersHandler remindersData,
        SleepHandler sleepData,
        BreaksHandler breaksData,
        ConsistencyHandler consistencyData)
    {
        ActivitiesData = activitiesData;
        RemindersData = remindersData;
        SleepData = sleepData;
        BreaksData = breaksData;
        ConsistencyData = consistencyData;
    }
    
    
    
    
}