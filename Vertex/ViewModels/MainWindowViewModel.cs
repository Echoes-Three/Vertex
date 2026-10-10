using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.MVVM;
using Vertex.Data.Handlers;
using Vertex.Data.Services;
using Vertex.Models.Entities;
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
    
    private readonly List<string> _meridiem = ["AM", "PM"];
    private int _meridiemIndex;
    private int _hour = 12;
    private int _minute = 59;
    
    private readonly int _today = (int)DateTime.Today.DayOfWeek;
    
    public RelayCommand OnSetTime { get; }
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
        
        OnSetTime = new RelayCommand(param => PickHourAction(param));
        
        WeakReferenceMessenger.Default.Register<SetSliceMessage>(this, (r, msg) =>
            InitializeSetHour(msg.Value));
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
    private void PickHourAction(object identifier)
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
        else if (id.StartsWith('T'))
        {
            _meridiemIndex = (_meridiemIndex + 1) % 2;
        }
        
        Hour = $"{_hour:D2}";
        Minute = $"{_minute:D2}";
        Meridiem =  _meridiem[_meridiemIndex];
        
        SetHour();
    }

    /*SetHour*/
    private void InitializeSetHour((object id, object x, object y) values)
    {
        var id = values.id.ToString();
        WindowX = (double)values.x + 590;
        WindowY = (double)values.y + 280;
        
        Entry = ActivitiesData.Activities!.FirstOrDefault(a => a.Id == id);
        
        var (hour, minute,  meridiem) = AngleConverters.AngleToHour(Entry.StartAngle[_today]);

        (Hour, _hour) = ($"{hour:D2}", hour);
        (Minute, _minute) = ($"{minute:D2}", minute);
        (Meridiem, _meridiemIndex) = (meridiem, meridiem == "AM" ? 0 : 1);

        SetHourVisibility = Visibility.Visible;
        _ = SetHourFocus();
    }

    public void SetHourEnter() => IsMouseOverSetHour = true;
    
    private void SetHourLeave()
    {
        SetHourVisibility = Visibility.Hidden;
        Entry = null;

        (Hour, _hour) = ("00", 0);
        (Minute, _minute) = ("00", 0);
        (Meridiem, _meridiemIndex) = ("AM", 0);
        
    }

    //FIX MINUTE DOWN PUSHING ACTIVITIES BEYOND 1 HOUR
    
    public async Task SetHourFocus()
    {
        IsMouseOverSetHour = false;
        
        await Task.Delay(1500);

        if (IsMouseOverSetHour)
        {
        }
        else
            SetHourLeave();

    }
    
    private void SetHour()
    {
        var degree = AngleConverters.HourToAngle(_hour, _minute, Meridiem);
        var Span = Entry!.Duration.Hours + Entry.Duration.Hours / 60.0;
        
        Entry.StartAngle[_today] = degree;
        Entry.EndAngle[_today] = degree + Span * 15;
        
        ActivitiesData.Serialize();
        WeakReferenceMessenger.Default.Send(new RebuildSlicesMessage());
    }
    
    
    public ActivityEntry? Entry
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    
    public string Hour
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    
    public string Minute
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    
    public string Meridiem
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public double WindowX
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = 0;

    public double WindowY
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = 0;

    public Visibility SetHourVisibility
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = Visibility.Hidden;

    public bool IsMouseOverSetHour
    {
        get;
        set
        {
            if (Equals(field, value)) return;
            field = value;
            OnPropertyChanged();
        }
    }
}