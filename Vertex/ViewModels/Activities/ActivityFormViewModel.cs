using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Threading;
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
    
    private List<bool> DaysOfWeek => [Sun, Mon, Tue, Wed, Thu, Fri, Sat];
    
    private readonly DispatcherTimer timer = new DispatcherTimer();
    
    private WindowMode _windowMode = WindowMode.Add;
    
    private int _hour;
    private int _minute;
    private int _colorIndex;
    
    private readonly Random _random = new();
    private CancellationTokenSource _cts = new();
    public ObservableCollection<Square> Squares { get; } = new();
    
    public RelayCommand OnSaveActivity { get; }
    public RelayCommand OnPickDuration { get; }
    public RelayCommand OnChangeColor { get; }
    
    public ActivityFormViewModel(ActivitiesHandler activitiesHandler)
    {
        _activitiesData = activitiesHandler;
        
        for (var i = 0; i != 40; i++)
        {
            Squares.Add(new Square());
        }
        
        OnSaveActivity = new RelayCommand(_ => SaveActivity(), _ => CanSaveActivity());
        OnPickDuration = new RelayCommand(param => PickDurationAction(param));
        OnChangeColor = new RelayCommand(_ => ChangeColor());
        
        SetColor();
    }

    /*Saving Activity*/
    public void LoadForEdit(string activityId)
    {
        _windowMode = WindowMode.Edit;
    
        var activityEntry = _activitiesData.Activities!.FirstOrDefault(x => x.Id == activityId);
        if (activityEntry == null) return;
    
        _colorIndex = activityEntry.Color;
        SetColor();
    
        ActivityId = activityEntry.Id;
        ActivityTitle = activityEntry.Title;
        ActivityContent = activityEntry.Content == "No Content" ? "" : activityEntry.Content ;
    
        var day = activityEntry.RepeatOn.ToBoolList();
        (Sun, Mon, Tue, Wed, Thu, Fri, Sat) = 
            (day.Sun, day.Mon, day.Tue, day.Wed, day.Thu, day.Fri, day.Sat);
    
        (_hour, DurationHour) = 
            (activityEntry.Duration.Hours, activityEntry.Duration.Hours.ToString("D2"));
        (_minute, DurationMinute) = 
            (activityEntry.Duration.Minutes, activityEntry.Duration.Minutes.ToString("D2"));

        OnSaveActivity.RaiseCanExecuteChanged();
    }
    
    private void SaveNewActivity()
    {
        var activity = new ActivityEntry
        {
            Color = _colorIndex,
            Title = ActivityTitle,
            Content = string.IsNullOrWhiteSpace(ActivityContent) ? "No Content" : ActivityContent,
            Id = Guid.NewGuid().ToString(),
            Duration = new TimeSpan(hours: _hour, minutes: _minute, seconds: 0),
            RepeatOn = DaysOfWeek.ToDayOfWeek(),
        };
        
        _activitiesData.Save(activity);
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
        activityEntry.Duration = new TimeSpan(hours: _hour, minutes: _minute, seconds: 0);
        activityEntry.RepeatOn = DaysOfWeek.ToDayOfWeek();
        
        WeakReferenceMessenger.Default.Send(new ActivityEditedMessage());
        WeakReferenceMessenger.Default.Send(new RebuildSlicesMessage());
        
        _activitiesData.Serialize();
    }
    
    private bool CanSaveActivity()
    {
        var daysOfWeek = DaysOfWeek.ToDayOfWeek();
        var duration = (_currentHourCount: _hour, _currentMinuteCount: _minute);
        
        var isTitleNotEmpty = ValidateActivity.Title(ActivityTitle);
        var isWeekDaySelected = ValidateActivity.WeekDay(DaysOfWeek);
        var isDurationValid = ValidateActivity.Duration(_activitiesData, daysOfWeek, duration, ActivityId);
       
        var canAdd = isTitleNotEmpty.IsValid && isWeekDaySelected.IsValid && isDurationValid.IsValid;
        
        var parts = new List<string> {isTitleNotEmpty.Message, isWeekDaySelected.Message, isDurationValid.Message}
            .Where(e => !string.IsNullOrWhiteSpace(e));
        
        WarningMessages = string.Join("\n", parts);
        ShowWarning = canAdd;
        return canAdd;
    }
    private void SaveActivity()
    {
        if (_windowMode == WindowMode.Add)
            SaveNewActivity();
        else
            SaveEditActivity();
    }
    
    public string WarningMessages
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
            TitleLimitIndicator = (25 - field.Length).ToString();
            OnPropertyChanged();
            OnSaveActivity.RaiseCanExecuteChanged();
        }
    } = "";
    public string ActivityContent
    {
        get;
        set
        {
            field = CharacterLimiter.LimitActivityContent(ref value);
            ContentLimitIndicator = (500 - field.Length).ToString();
            OnPropertyChanged();
            OnSaveActivity.RaiseCanExecuteChanged();
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
        
        (Sun, Mon, Tue, Wed, Thu, Fri, Sat) =
            (false, false, false, false, false, false, false);
        (_hour, DurationHour) = (0, "00");
        (_minute, DurationMinute) = (0, "00");
        
        SetColor();
    }
    
    /*Color Picking on ActivityForm*/
    private void ChangeColor()
    {
        _colorIndex = _colorIndex == Colors.Palette.Count - 1 ? 0 : _colorIndex + 1;
        SetColor();
    }
    private void SetColor() =>
        SelectedColor = Colors.Palette[_colorIndex];
    
    public Brush SelectedColor
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public string ContentLimitIndicator
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = "500";
    public string TitleLimitIndicator
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = "25";
    
    /*Pick Duration*/
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
            OnSaveActivity.RaiseCanExecuteChanged();
        }
    } = "00";
    public string DurationMinute
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveActivity.RaiseCanExecuteChanged();
        }
    } = "00";
    
    
    /*Remaining Properties*/
    
    public bool Sun
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveActivity.RaiseCanExecuteChanged();
        }
    }

    public bool Mon
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveActivity.RaiseCanExecuteChanged();
        }
    }

    public bool Tue
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveActivity.RaiseCanExecuteChanged();
        }
    }

    public bool Wed
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveActivity.RaiseCanExecuteChanged();
        }
    }

    public bool Thu
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveActivity.RaiseCanExecuteChanged();
        }
    }

    public bool Fri
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveActivity.RaiseCanExecuteChanged();
        }
    }

    public bool Sat
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveActivity.RaiseCanExecuteChanged();
        }
    }
    
    
    /*Hover Animation*/
    public void  SetHover(bool hovered)
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        
        foreach (var square in Squares)
        {
            var delay = _random.Next(0, 300); 
            _ = ApplyAfterDelay(square, hovered, delay, _cts.Token);
        }
        
    }
    private async Task ApplyAfterDelay(Square square, bool value, int delay, CancellationToken token)
    {
        try
        {
            await Task.Delay(delay, token);
            square.IsActive = value;
        }
        catch (OperationCanceledException) { }
    }
    
    public void ResetAnimation()
    {
        MainWidth = 0;
        timer.Tick -= Timer_Tick;
        timer.Stop();
    }

    public void StartAnimation()
    {
        timer.Interval = TimeSpan.FromMicroseconds(400);
        timer.Tick += Timer_Tick;
        timer.Start();
    }
    
    private void Timer_Tick(object? sender, EventArgs e)
    {
        if(MainWidth != 340)
            MainWidth++;
    }
    
    public int MainWidth
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = 0;
}