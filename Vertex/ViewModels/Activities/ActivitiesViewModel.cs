using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Data.Handlers;
using Vertex.Data.Services;
using Vertex.Models.Entities;
using Vertex.Models.Entities.Entry;
using Vertex.Models.Enums;
using Vertex.MVVM;
using Vertex.Views.Activities;

namespace Vertex.ViewModels.Activities;

public class ActivitiesViewModel : ViewModelBase
{
    private ActivitiesHandler ActivitiesData { get; set; }
    
    private ObservableCollection<ActivityItemViewModel> _currentDayActivities;

    public ObservableCollection<ActivityItemViewModel> ActivitiesForToday
    {
        get => _currentDayActivities;
        set
        {
            _currentDayActivities = value;
            OnPropertyChanged();
        }
    }

    
    private ObservableCollection<ActivityItemViewModel> _allActivities;

    public ObservableCollection<ActivityItemViewModel> ActivitiesNotForToday
    {
        get => _allActivities;
        set
        {
            _allActivities = value;
            OnPropertyChanged();
        }
    }

    private WindowMode _windowMode = WindowMode.Add;
    private Window? ActivityWindowView { get; set; }
    private int _currentHourCount = 00;
    private int _currentMinuteCount = 00;
    private int _currentColorGroupIndex = 0;

    private List<bool> DaysOfWeek => [Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday];
    private List<Brush> CurrentColorGroup => ActivityColors.Categories[_currentColorGroupIndex];
    
    public RelayCommand OnAddActivityView { get; }
    public RelayCommand OnColorGroupSwap { get; }
    public RelayCommand OnSelectColor { get; }
    public RelayCommand OnSaveAction { get; }


    public ActivitiesViewModel(ActivitiesHandler activitiesHandler)
    {
        ActivitiesData = activitiesHandler;

        ActivitiesForToday = new ObservableCollection<ActivityItemViewModel>(
            ActivitiesData.Activities!.Where(x => x!.RepeatOn.Contains(DateTime.Today.DayOfWeek))
                .Select(x => new ActivityItemViewModel(x)));

        ActivitiesNotForToday = new ObservableCollection<ActivityItemViewModel>(
            ActivitiesData.Activities!.Where(x => !x!.RepeatOn.Contains(DateTime.Today.DayOfWeek))
                .Select(x => new ActivityItemViewModel(x)));

        activitiesHandler.Activities!.CollectionChanged += (s, e) =>
        {
            if (e.NewItems != null)
                foreach (ActivityEntry entry in e.NewItems)
                {
                    ActivitiesNotForToday.Add(new ActivityItemViewModel(entry));
                    if (entry.RepeatOn.Contains(DateTime.Today.DayOfWeek))
                        ActivitiesForToday.Add(new ActivityItemViewModel(entry));
                }
            
            if (e.OldItems != null)
                foreach (ActivityEntry entry in e.OldItems)
                {
                    var vmOne = ActivitiesNotForToday.FirstOrDefault(x => x.EntryData!.Id == entry.Id);
                    if (vmOne != null)
                        ActivitiesNotForToday.Remove(vmOne);
                    var vmTwo = ActivitiesForToday.FirstOrDefault(x => x.EntryData!.Id == entry.Id);
                    if (vmTwo != null)
                        ActivitiesForToday.Remove(vmTwo);
                }
        };
        
        WeakReferenceMessenger.Default.Register<DeleteActivityMessage>(this, (r, msg) =>
            DeleteActivity(msg.Value));

        WeakReferenceMessenger.Default.Register<EditActivityMessage>(this, (r, msg) =>
            EditActivity(msg.Value));

        WeakReferenceMessenger.Default.Register<MarkActivityAsDoneMessage>(this, (r, msg) =>
            MarkActivityAsDone(msg.Value));
        
        OnAddActivityView = new RelayCommand(_ => ActivityWindow());
        OnColorGroupSwap = new RelayCommand(_ => SwapColorGroup());
        OnSelectColor = new RelayCommand(index => SetColor(index));
        OnSaveAction = new RelayCommand(_ => SaveAction(), _ => CanSaveAction());
        
        SetColorGroup();
    }

    /*Add/Edit Window Actions*/
    private void ActivityWindow()
    {
        if (ActivityWindowView == null)
        {
            ActivityWindowView = new Window
            {
                Title = "ActivityWindowView",
                DataContext = this,
                Owner = Application.Current.MainWindow,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Width = 650,
                Height = 200,
                Content = new ActivityViewWindow()
            };

            ActivityWindowView.Closed += (_, _) => ActivityWindowView = null;
            ActivityWindowView.ShowDialog();
        }
    }

    private bool CanSaveAction() =>
        ActivityTitle != "" 
            && ActivityRepeat.HasMinimumValue(DaysOfWeek)
            && ((_currentHourCount == 0 && _currentMinuteCount >= 15)
                || (_currentHourCount > 0 && _currentMinuteCount >= 0));
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
            Color = (_currentColorGroupIndex, _currentColorIndex),
            Title = ActivityTitle,
            Content = ActivityContent,
            Id = Guid.NewGuid().ToString(),
            Done = false,
            DurationHours = new TimeSpan(hours: _currentHourCount, minutes: _currentMinuteCount, seconds: 0),
            RepeatOn = DaysOfWeek.ToDayOfWeek()
        };

        ActivitiesData.Save(activity);
        ActivityWindowView!.Close();

        CleanActivityWindowFields();
    }
    private void SaveEditActivity()
    {
        var activityEntry = ActivitiesData.Activities!.FirstOrDefault(x => x.Id == ActivityId);
        if (activityEntry == null) return;

        activityEntry.Id = ActivityId;
        activityEntry.Color = (_currentColorGroupIndex, _currentColorIndex);
        activityEntry.Title = ActivityTitle;
        activityEntry.Content = ActivityContent;
        activityEntry.Done = false;
        activityEntry.DurationHours = new TimeSpan(hours: _currentHourCount, minutes: _currentMinuteCount, seconds: 0);
        activityEntry.RepeatOn = DaysOfWeek.ToDayOfWeek();
        
        ActivitiesData.Serialize();
        ActivityWindowView!.Close();
        
        CleanActivityWindowFields();
        ReloadCollection();
    }
    
    
    /*Actions on CurrentDayActivities*/
    private void MarkActivityAsDone(string activityId)
    {
        var activityEntry = ActivitiesData.Activities!.FirstOrDefault(x => x.Id == activityId);
        if (activityEntry == null) return;
        activityEntry.Done = true;

        ReloadCollection();
    }
    private void EditActivity(string activityId)
    {
        _windowMode = WindowMode.Edit;
        
        var activityEntry = ActivitiesData.Activities!.FirstOrDefault(x => x.Id == activityId);
        if (activityEntry == null) return;

        _currentColorGroupIndex = activityEntry.Color.GroupIndex;
        _currentColorIndex = activityEntry.Color.ColorIndex;
        SetColorGroup();

        ActivityId = activityEntry.Id;
        ActivityTitle = activityEntry.Title;
        ActivityContent = activityEntry.Content;
        
        var day =  activityEntry.RepeatOn.ToBoolList();
        (Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday) = 
            (day.Mon, day.Tue, day.Wed, day.Thu, day.Fri, day.Sat, day.Sun);
        
        (_currentHourCount, CurrentDurationHour) = 
            (activityEntry.DurationHours.Hours, activityEntry.DurationHours.Hours.ToString("D2"));
        (_currentMinuteCount, CurrentDurationMinute) = 
            (activityEntry.DurationHours.Minutes, activityEntry.DurationHours.Minutes.ToString("D2"));

        ActivityWindow();
    }
    private void DeleteActivity(string activityId)
    {
        var activityEntry = ActivitiesData.Activities!.FirstOrDefault(x => x.Id == activityId);
        if (activityEntry == null) return;
        ActivitiesData.Delete(activityEntry!);
    }
    
    
    /*Helper methods*/
    public void CleanActivityWindowFields()
    {
        ActivityId = "";
        _windowMode = WindowMode.Add;
        ActivityTitle = "";
        ActivityContent = "";
        _currentColorGroupIndex = 0;
        CurrentColorIndex = 0;
        SetColorGroup();
        SetColor("0");
        (Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday) =
            (true, true, true, true, true, true, true);
        (_currentHourCount, CurrentDurationHour) = (0, "00");
        (_currentMinuteCount, CurrentDurationMinute) = (0, "00");
    }
    private void ReloadCollection()
    {
        ActivitiesForToday = null;
        ActivitiesForToday = new ObservableCollection<ActivityItemViewModel>(
            ActivitiesData.Activities!.Where(x => x!.RepeatOn.Contains(DateTime.Today.DayOfWeek))
                .Select(x => new ActivityItemViewModel(x)));
        
        ActivitiesNotForToday = new ObservableCollection<ActivityItemViewModel>(
            ActivitiesData.Activities!.Where(x => !x!.RepeatOn.Contains(DateTime.Today.DayOfWeek))
                .Select(x => new ActivityItemViewModel(x)));
    }
    
    
    /*Color Picking on ActivityViewWindow*/
    private void SwapColorGroup()
    {
        _currentColorGroupIndex = (_currentColorGroupIndex + 1) % 6;
        SetColorGroup();
    }
    private void SetColorGroup()
    {
        var g = CurrentColorGroup;
        (ColorOne, ColorTwo, ColorThree, ColorFour, ColorFive, ColorSix) = (g[0], g[1], g[2], g[3], g[4], g[5]);
        SelectedColor = g[CurrentColorIndex];
    }
    private void SetColor(object index) =>
        (SelectedColor, CurrentColorIndex) = (CurrentColorGroup[Convert.ToInt32(index)], Convert.ToInt32(index));
    
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
    
    private bool _showAllActivities;

    public bool ShowAllActivities
    {
        get => _showAllActivities;
        set
        {
            _showAllActivities = value;
            OnPropertyChanged();
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

    private int _currentColorIndex;

    public int CurrentColorIndex
    {
        get => _currentColorIndex;
        set
        {
            _currentColorIndex = value;
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

    private Brush _colorOne;

    public Brush ColorOne
    {
        get => _colorOne;
        set
        {
            _colorOne = value;
            OnPropertyChanged();
        }
    }

    private Brush _colorTwo;

    public Brush ColorTwo
    {
        get => _colorTwo;
        set
        {
            _colorTwo = value;
            OnPropertyChanged();
        }
    }

    private Brush _colorThree;

    public Brush ColorThree
    {
        get => _colorThree;
        set
        {
            _colorThree = value;
            OnPropertyChanged();
        }
    }

    private Brush _colorFour;

    public Brush ColorFour
    {
        get => _colorFour;
        set
        {
            _colorFour = value;
            OnPropertyChanged();
        }
    }

    private Brush _colorFive;

    public Brush ColorFive
    {
        get => _colorFive;
        set
        {
            _colorFive = value;
            OnPropertyChanged();
        }
    }

    private Brush _colorSix;

    public Brush ColorSix
    {
        get => _colorSix;
        set
        {
            _colorSix = value;
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

    private string _activityTitle = "";

    public string ActivityTitle
    {
        get => _activityTitle;
        set
        {
            _activityTitle = CharacterLimiter.LimitActivityTitle(value);
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
            _activityContent = value;
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