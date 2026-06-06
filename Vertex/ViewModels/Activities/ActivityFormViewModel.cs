using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Data.Handlers;
using Vertex.Data.Services;
using Vertex.Models.Entities;
using Vertex.MVVM;
using Colors = Vertex.Data.Services.Colors;

namespace Vertex.ViewModels.Activities;

public class ActivityFormViewModel : ViewModelBase
{
    private readonly ActivitiesHandler _activitiesData;
    
    private List<bool> DaysOfWeek => [Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday];
    
    private WindowMode _windowMode = WindowMode.Add;
    private int _hourCount;
    private int _minuteCount;
    
    private int _colorIndex;
    private readonly int _palletLastIndex = Colors.Palette.Count - 1;
    
    private Action? _closeWindow;
    public void SetCloseAction(Action close) => _closeWindow = close;
    
    public RelayCommand OnSaveAction { get; }
    
    public ActivityFormViewModel(ActivitiesHandler activitiesHandler)
    {
        _activitiesData = activitiesHandler;
        
        OnSaveAction = new RelayCommand(_ => SaveAction(), _ => CanSaveAction());
        
        SetColor();
        ContentLimitIndicator = Colors.GetBrush("#ea163b");
        TitleLimitIndicator = Colors.GetBrush("#ea163b");
    }
    
    /*Saving Activity*/
    public void LoadForEdit(string activityId)
    {
        _windowMode = WindowMode.Edit;
    
        var activityEntry = _activitiesData.Activities!.FirstOrDefault(x => x.Id == activityId);
        if (activityEntry == null) return;
    
        _colorIndex = activityEntry.Color;
        ColorNumber = $"{activityEntry.Color + 1}";
        SetColor();
    
        ActivityId = activityEntry.Id;
        ActivityTitle = activityEntry.Title;
        ActivityContent = activityEntry.Content == "No Content" ? "" : activityEntry.Content ;
    
        var day = activityEntry.RepeatOn.ToBoolList();
        (Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday) = 
            (day.Sun, day.Mon, day.Tue, day.Wed, day.Thu, day.Fri, day.Sat);
    
        (_hourCount, DurationHour) = 
            (activityEntry.Duration.Hours, activityEntry.Duration.Hours.ToString("D2"));
        (_minuteCount, DurationMinute) = 
            (activityEntry.Duration.Minutes, activityEntry.Duration.Minutes.ToString("D2"));

        OnSaveAction.RaiseCanExecuteChanged();
    }
    
    private void SaveNewActivity()
    {
        var activity = new ActivityEntry
        {
            Color = _colorIndex,
            Title = ActivityTitle,
            Content = string.IsNullOrWhiteSpace(ActivityContent) ? "No Content" : ActivityContent,
            Id = Guid.NewGuid().ToString(),
            Duration = new TimeSpan(hours: _hourCount, minutes: _minuteCount, seconds: 0),
            RepeatOn = DaysOfWeek.ToDayOfWeek(),
        };
        
        _activitiesData.Save(activity);
        _closeWindow?.Invoke();
        CleanFields();
        
    }
    private void SaveEditActivity()
    {
        var activityEntry = _activitiesData.Activities!.FirstOrDefault(x => x.Id == ActivityId);
        if (activityEntry == null) return;
        
        activityEntry.Id = ActivityId;
        activityEntry.Color = _colorIndex;
        activityEntry.Title = ActivityTitle;
        activityEntry.Content = string.IsNullOrWhiteSpace(ActivityContent) ? "No Content" : ActivityContent;
        activityEntry.Duration = new TimeSpan(hours: _hourCount, minutes: _minuteCount, seconds: 0);
        activityEntry.RepeatOn = DaysOfWeek.ToDayOfWeek();
        
        WeakReferenceMessenger.Default.Send(new ActivityEditedMessage());
        WeakReferenceMessenger.Default.Send(new RebuildSlicesMessage());
        
        _activitiesData.Serialize();
        _closeWindow?.Invoke();
    }
    
    private bool CanSaveAction()
    {
        var daysOfWeek = DaysOfWeek.ToDayOfWeek();
        var duration = (_currentHourCount: _hourCount, _currentMinuteCount: _minuteCount);
        
        var isTitleNotEmpty = ValidateActivity.Title(ActivityTitle);
        var isWeekDaySelected = ValidateActivity.WeekDay(DaysOfWeek);
        var isDurationValid = ValidateActivity.Duration(_activitiesData, daysOfWeek, duration, ActivityId);
       
        var canAdd = isTitleNotEmpty.IsValid && isWeekDaySelected.IsValid && isDurationValid.IsValid;
        
        var parts = new List<string> {isTitleNotEmpty.Message, isWeekDaySelected.Message, isDurationValid.Message}
            .Where(e => !string.IsNullOrWhiteSpace(e));
        
        WarningMessages = parts.Any() ? string.Join("\n", parts) : "No warning." ;
        WarningColor = canAdd ? Colors.GetBrush("#C3FE0C") : Colors.GetBrush("#ea163b");
        ShowWarning = canAdd;
        return canAdd;
    }
    private void SaveAction()
    {
        if (_windowMode == WindowMode.Add)
            SaveNewActivity();
        else
            SaveEditActivity();
    }

    private static SolidColorBrush LimitColor(int length, int limit) =>
        (limit, length) switch
        {
            var (lim, len) when lim - len > lim * 0.2 => Colors.GetBrush("#C3FE0C"),
            var (lim, len) when len == lim => Colors.GetBrush("#ea163b"),
            _ => Colors.GetBrush("#0c4af7")
        };
    
    public string WarningMessages
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public Brush? WarningColor
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public bool ShowWarning
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    private string ActivityId
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public string ActivityTitle
    {
        get;
        set
        {
            field = CharacterLimiter.LimitActivityTitle(ref value);
            TitleLimitIndicator = LimitColor(field.Length, 25);
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = "";
    public string ActivityContent
    {
        get;
        set
        {
            field = CharacterLimiter.LimitActivityContent(ref value);
            ContentLimitIndicator = LimitColor(field.Length, 500);
            OnPropertyChanged();
        }
    } = "";
    
    
    /*Helper Methods*/
    public void CleanFields()
    {
        _windowMode = WindowMode.Add;
        
        ActivityId = "";
        ActivityTitle = "";
        ActivityContent = "";
        
        _colorIndex = 0;
        ColorNumber = "1";
        
        (Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday) =
            (true, true, true, true, true, true, true);
        (_hourCount, DurationHour) = (0, "00");
        (_minuteCount, DurationMinute) = (0, "00");
        
        SetColor();
    }
    
    /*Color Picking on AddActivityWindow*/
    public void ColorIndexUp()
    {
        _colorIndex = _colorIndex == _palletLastIndex ? 0 : _colorIndex + 1;
        ColorNumber = $"{_colorIndex + 1}";
        SetColor();
    }
    public void ColorIndexDown()
    { 
        _colorIndex = _colorIndex == 0 ? _palletLastIndex : _colorIndex - 1;
        ColorNumber = $"{_colorIndex + 1}";
        SetColor();
    }
    private void SetColor() =>
        SelectedColor = Colors.Palette[_colorIndex];
    
    public string ColorNumber
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = "0";
    public Brush SelectedColor
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public Brush? ContentLimitIndicator
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } 
    public Brush? TitleLimitIndicator
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    
    /*Duration scroll behavior on AddActivityWindow*/
    public void DurationHourUp()
    {
        _hourCount = _hourCount == 12 ? 0 : _hourCount + 1;
        DurationHour = $"{_hourCount:D2}";
    }
    public void DurationHourDown()
    {
        _hourCount = _hourCount == 0 ? 12 : _hourCount - 1;
        DurationHour = $"{_hourCount:D2}";
    }
    public void DurationMinuteUp()
    {
        _minuteCount = _minuteCount == 59 ? 0 : _minuteCount + 1;
        DurationMinute = $"{_minuteCount:D2}";
    }
    public void DurationMinuteDown()
    { 
        _minuteCount = _minuteCount == 0 ? 59 : _minuteCount - 1; 
        DurationMinute = $"{_minuteCount:D2}";
    }
    
    public string DurationHour
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = "00";
    public string DurationMinute
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = "00";
    
    
    /*Remaining Properties*/
    
    public bool Sunday
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = true;

    public bool Monday
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = true;

    public bool Tuesday
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = true;

    public bool Wednesday
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = true;

    public bool Thursday
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = true;

    public bool Friday
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = true;

    public bool Saturday
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = true;
    
}