using System.Windows.Media;
using Vertex.Models.EnumDefinitions;
using Vertex.Models.UserData.DataHandling;
using Vertex.Models.UserData.Entry;
using Vertex.MVVM;

namespace Vertex.ViewModels;

public class DailyActivitiesViewModel : ViewModelBase
{
    private ActivitiesHandler _activitiesHandler;
    
    public DailyActivitiesViewModel()
    {
        var dailyData = new ActivitiesHandler();
        _activitiesHandler = dailyData;
    }
    
    public void OnAddActivity()
    {
        var activities = new ActivityEntry(
            ActivityId,
            ActivityColor,
            ActivityName,
            ActivityCompleted,
            ActivityDurationHour,
            ActivityRequiredEnergy,
            ActivityExpectedEnjoyment,
            ActivityPlacementOrder
        );

        _activitiesHandler.Save(activities);
    }
    
    public bool CanAddActivity()
    {
        return true;
    }

    
    private string _activityId;

    public string ActivityId
    {
        get => _activityId;
        set
        {
            _activityId = value;
            OnPropertyChanged();
        }
    }

    private Color _activityColor;

    public Color ActivityColor
    {
        get => _activityColor;
        set
        {
            _activityColor = value;
            OnPropertyChanged();
        }
    }

    private string _activityName;

    public string ActivityName
    {
        get => _activityName;
        set
        {
            _activityName = value;
            OnPropertyChanged();
        }
    }

    private bool _activityCompleted;

    public bool ActivityCompleted
    {
        get => _activityCompleted;
        set
        {
            _activityCompleted = value;
            OnPropertyChanged();
        }
    }

    private TimeSpan _activityDurationHour;

    public TimeSpan ActivityDurationHour
    {
        get => _activityDurationHour;
        set
        {
            _activityDurationHour = value;
            OnPropertyChanged();
        }
    }

    private RequiredEnergy _activityRequiredEnergy;

    public RequiredEnergy ActivityRequiredEnergy
    {
        get => _activityRequiredEnergy;
        set
        {
            _activityRequiredEnergy = value;
            OnPropertyChanged();
        }
    }

    private ExpectedEnjoyment _activityExpectedEnjoyment;

    public ExpectedEnjoyment ActivityExpectedEnjoyment
    {
        get => _activityExpectedEnjoyment;
        set
        {
            _activityExpectedEnjoyment = value;
            OnPropertyChanged();
        }
    }

    private int _activityPlacementOrder;

    public int ActivityPlacementOrder
    {
        get => _activityPlacementOrder;
        set
        {
            _activityPlacementOrder = value;
            OnPropertyChanged();
        }
    }
    
}