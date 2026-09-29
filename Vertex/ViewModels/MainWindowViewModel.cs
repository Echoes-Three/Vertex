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

    private void SwapToggle(string tab)
    {
        switch (tab)
        {
            case "Activity":
                ReminderIsVisible = false;
                AddIsVisible = false;
                break;
            case "Reminder":
                ActivityIsVisible = false;
                AddIsVisible = false;
                break;
            case "Add":
                ActivityIsVisible = false;
                ReminderIsVisible = false;
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
                case true when !value && ReminderIsVisible || AddIsVisible:
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
                case true when !value && ActivityIsVisible || AddIsVisible:
                    field = value;
                    break;
            }
            OnPropertyChanged();
            
        }
    }
    
    public bool AddIsVisible
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
}