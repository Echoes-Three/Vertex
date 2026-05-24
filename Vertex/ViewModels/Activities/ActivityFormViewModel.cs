using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Data.Handlers;
using Vertex.Data.Services;
using Vertex.Models.Entities;
using Vertex.Models.Entities.Entry;
using Vertex.Models.Enums;
using Vertex.MVVM;

namespace Vertex.ViewModels.Activities;

public class ActivityFormViewModel : ViewModelBase
{
    private readonly ActivitiesHandler _activitiesData;
    
    private List<bool> DaysOfWeek => [Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday];
    
    private WindowMode _windowMode = WindowMode.Add;
    private int _currentHourCount = 00;
    private int _currentMinuteCount = 00;
    
    private int _colorIndex = 0;
    private readonly int _lastIndex = ActivityColors.Palette.Count - 1;
    
    private Action? _closeWindow;
    public void SetCloseAction(Action close) => _closeWindow = close;
    
    public RelayCommand OnSaveAction { get; }
    
    public ActivityFormViewModel(ActivitiesHandler activitiesHandler)
    {
        _activitiesData = activitiesHandler;
        
        OnSaveAction = new RelayCommand(_ => SaveAction(), _ => CanSaveAction());
        
        SetColors();
    }
    
    private bool CanSaveAction()
    {
        var daysOfWeek = DaysOfWeek.ToDayOfWeek();
        var duration = (_currentHourCount, _currentMinuteCount);
        
        var isTitleNotEmpty = ValidateActivity.Title(ActivityTitle);
        var isWeekDaySelected = ValidateActivity.WeekDay(DaysOfWeek);
        var isDurationValid = ValidateActivity.Duration(_activitiesData, daysOfWeek, duration, ActivityId);
       
        var canAdd = isTitleNotEmpty.IsValid && isWeekDaySelected.IsValid && isDurationValid.IsValid;
        
        var parts = new List<string> {isTitleNotEmpty.Message, isWeekDaySelected.Message, isDurationValid.Message}
            .Where(e => !string.IsNullOrWhiteSpace(e));
        
        WarningMessages = parts.Any() ? string.Join("\n", parts) : "No warning." ;
        
        WarningColor = canAdd ? ActivityColors.GetBrush("#C3FE0C") : ActivityColors.GetBrush("#ea163b");

        return canAdd;
    }
    private void SaveAction()
    {
        if (_windowMode == WindowMode.Add)
            SaveNewActivity();
        else
            SaveEditActivity();
    }
    
    private void SaveNewActivity()
    {
        var activity = new ActivityEntry
        {
            Color = _colorIndex,
            Title = ActivityTitle,
            Content = ActivityContent,
            Id = Guid.NewGuid().ToString(),
            Done = false,
            Duration = new TimeSpan(hours: _currentHourCount, minutes: _currentMinuteCount, seconds: 0),
            RepeatOn = DaysOfWeek.ToDayOfWeek()
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
        activityEntry.Content = ActivityContent;
        activityEntry.Done = false;
        activityEntry.Duration = new TimeSpan(hours: _currentHourCount, minutes: _currentMinuteCount, seconds: 0);
        activityEntry.RepeatOn = DaysOfWeek.ToDayOfWeek();
        
        _activitiesData.Serialize();
        _closeWindow?.Invoke();
        
        WeakReferenceMessenger.Default.Send(new ActivityEditedMessage());
        WeakReferenceMessenger.Default.Send(new RebuildSlicesMessage());
    }
    public void LoadForEdit(string activityId)
    {
        _windowMode = WindowMode.Edit;
    
        var activityEntry = _activitiesData.Activities!.FirstOrDefault(x => x.Id == activityId);
        if (activityEntry == null) return;
    
        _colorIndex = activityEntry.Color;
        SetColors();
    
        ActivityId = activityEntry.Id;
        ActivityTitle = activityEntry.Title;
        ActivityContent = activityEntry.Content;
    
        var day = activityEntry.RepeatOn.ToBoolList();
        (Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday) = 
            (day.Sun, day.Mon, day.Tue, day.Wed, day.Thu, day.Fri, day.Sat);
    
        (_currentHourCount, CurrentDurationHour) = 
            (activityEntry.Duration.Hours, activityEntry.Duration.Hours.ToString("D2"));
        (_currentMinuteCount, CurrentDurationMinute) = 
            (activityEntry.Duration.Minutes, activityEntry.Duration.Minutes.ToString("D2"));

        OnSaveAction.RaiseCanExecuteChanged();
    }
    
    /*Helper Methods*/
    public void CleanFields()
    {
        ActivityId = "";
        _windowMode = WindowMode.Add;
        ActivityTitle = "";
        ActivityContent = "";
        _colorIndex = 0;
        SetColors();
        (Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday) =
            (true, true, true, true, true, true, true);
        (_currentHourCount, CurrentDurationHour) = (0, "00");
        (_currentMinuteCount, CurrentDurationMinute) = (0, "00");
    }
    private void CountContentLimit(int length) =>
        ContentLimitCounter = (500 - length).ToString();
    private void CountTitleLimit(int length) => 
        TitleLimitCounter = (30 - length).ToString();
    
    
    /*Color Picking on AddActivityWindow*/
    public void ColorIndexUp()
    {
        if (_colorIndex == _lastIndex)
            _colorIndex = 0;
        else
            _colorIndex++;

        SetColors();
    }
    public void ColorIndexDown()
    { 
        if (_colorIndex == 0)
            _colorIndex = _lastIndex;
        else
            _colorIndex--;

        SetColors();
    }
    private void SetColors() =>
        SelectedColor = ActivityColors.Palette[_colorIndex];
    
    
    /*Methods responsible for duration scroll behavior*/
    public void DurationHourUp()
    {
        if (_currentHourCount == 12)
            _currentHourCount = 0;
        else
            _currentHourCount++;

        CurrentDurationHour = $"{_currentHourCount:D2}";
    }
    public void DurationHourDown()
    {
        if (_currentHourCount == 0)
            _currentHourCount = 12;
        else
            _currentHourCount--;

        CurrentDurationHour = $"{_currentHourCount:D2}";
    }
    public void DurationMinuteUp()
    {
        if (_currentMinuteCount == 59)
            _currentMinuteCount = 0;
        else
            _currentMinuteCount++;

        CurrentDurationMinute = $"{_currentMinuteCount:D2}";
    }
    public void DurationMinuteDown()
    {
        if (_currentMinuteCount == 0)
            _currentMinuteCount = 59;
        else
            _currentMinuteCount--;

        CurrentDurationMinute = $"{_currentMinuteCount:D2}";
    }
    
    /*Full Properties*/
    private bool _sunday = true;

    public bool Sunday
    {
        get => _sunday;
        set
        {
            _sunday = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    }
    
    private bool _monday = true;

    public bool Monday
    {
        get => _monday;
        set
        {
            _monday = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    }

    private bool _tuesday = true;

    public bool Tuesday
    {
        get => _tuesday;
        set
        {
            _tuesday = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    }

    private bool _wednesday = true;

    public bool Wednesday
    {
        get => _wednesday;
        set
        {
            _wednesday = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    }

    private bool _thursday = true;

    public bool Thursday
    {
        get => _thursday;
        set
        {
            _thursday = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    }

    private bool _friday = true;

    public bool Friday
    {
        get => _friday;
        set
        {
            _friday = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    }

    private bool _saturday = true;

    public bool Saturday
    {
        get => _saturday;
        set
        {
            _saturday = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    }
    
    /*Full Properties*/

    private string _warningMessages;

    public string WarningMessages
    {
        get => _warningMessages;
        set
        {
            _warningMessages = value;
            OnPropertyChanged();
        }
    }

    
    private Brush? _warningColor;

    public Brush? WarningColor
    {
        get => _warningColor;
        set
        {
            _warningColor = value;
            OnPropertyChanged();
        }
    }
    
    private string _contentLimitCounter = "500";

    public string ContentLimitCounter
    {
        get => _contentLimitCounter;
        set
        {
            _contentLimitCounter = value;
            OnPropertyChanged();
        }
    }

    private string _titleLimitCounter = "22";

    public string TitleLimitCounter
    {
        get => _titleLimitCounter;
        set
        {
            _titleLimitCounter = value;
            OnPropertyChanged();
        }
    }
    
    private Brush _selectedColor;

    public Brush SelectedColor
    {
        get => _selectedColor;
        set
        {
            _selectedColor = value;
            OnPropertyChanged();
        }
    }
    
    private string _currentDurationHour = "00";

    public string CurrentDurationHour
    {
        get => _currentDurationHour;
        set
        {
            _currentDurationHour = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    }

    private string _currentDurationMinute = "00";

    public string CurrentDurationMinute
    {
        get => _currentDurationMinute;
        set
        {
            _currentDurationMinute = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
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
    
    private string _activityTitle = "";

    public string ActivityTitle
    {
        get => _activityTitle;
        set
        {
            _activityTitle = CharacterLimiter.LimitActivityTitle(value);
            CountTitleLimit(_activityTitle.Length);
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    }
    
    private string _activityContent = "";

    public string ActivityContent
    {
        get => _activityContent;
        set
        {
            _activityContent = CharacterLimiter.LimitActivityContent(value);
            CountContentLimit(_activityContent.Length);
            OnPropertyChanged();
        }
    }
}