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
    public ActivityFormViewModel ActivityFormVM { get; }
    public RemindersViewModel RemindersVM { get; }
    public  DonutGraphViewModel DonutGraphVM { get; }
    
    private int _hour;
    private int _minute;

    public MainWindowViewModel(
        ActivitiesHandler activitiesData,
        RemindersHandler remindersData,
        
        ActivitiesViewModel activitiesViewModel,
        ActivityFormViewModel activityFormViewModel,
        RemindersViewModel remindersViewModel,
        DonutGraphViewModel  donutGraphViewModel)
    {
        ActivitiesData = activitiesData;
        RemindersData = remindersData;
        
        ActivitiesVM = activitiesViewModel;
        ActivityFormVM = activityFormViewModel;
        RemindersVM = remindersViewModel;
        DonutGraphVM = donutGraphViewModel;
    }

    private void SwapToggle(string tab)
    {
        switch (tab)
        {
            case "Activity":
                (ReminderIsVisible, FormIsVisible) = (false, false);
                break;
            case "Reminder":
                (ActivityIsVisible, FormIsVisible) = (false, false);
                break;
            case "Add":
                (ActivityIsVisible, ReminderIsVisible) = (false, false);
                break;
        }
    }
    
    public bool ActivityIsVisible
    {
        get;
        set
        {
            switch (field)
            {
                case false when value:
                    field = value;
                    SwapToggle("Activity");
                    break;
                case true when !value && ReminderIsVisible || FormIsVisible:
                    field = value;
                    break;
            }
            OnPropertyChanged();
        }
    } = true;
    
    public bool ReminderIsVisible
    {
        get;
        set
        {
            switch (field)
            {
                case false when value:
                    field = value;
                    SwapToggle("Reminder");
                    break;
                case true when !value && ActivityIsVisible || FormIsVisible:
                    field = value;
                    break;
            }
            OnPropertyChanged();
            
        }
    }
    
    public bool FormIsVisible
    {
        get;
        set
        {
            switch (field)
            {
                case false when value:
                    field = value;
                    SwapToggle("Add");
                    break;
                case true when !value && ActivityIsVisible || ReminderIsVisible:
                    field = value;
                    break;
            }
            OnPropertyChanged();
            
        }
    }

    public bool SwapForm
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    
    /*Duration Setter*/
    private void PickDurationAction(object identifier)
    {
        var id = identifier.ToString();
        
        if (id!.StartsWith('H'))
        {
            if (id.EndsWith('U'))
                _hour = _hour == 12 ? 0 : _hour + 1;
            else
                _hour = _hour == 0 ? 12 : _hour - 1;
        }
        else if (id.StartsWith('M'))
        {
            if (id.EndsWith('U'))
                _minute = _minute == 59 ? 0 : _minute + 1;
            else
                _minute = _minute == 0 ? 59 : _minute - 1; 
        }
        
        DurationHour = $"{_hour:D2}";
        DurationMinute = $"{_minute:D2}";
    }
    
    public string DurationHour
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = "00";
    public string DurationMinute
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = "00";
    
    public string DurationMeridiem
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = "AM";
}