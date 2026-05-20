using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Data.Handlers;
using Vertex.Models.Entities;
using Vertex.Models.Entities.Entry;
using Vertex.MVVM;
using Vertex.ViewModels.Activities;

namespace Vertex.ViewModels.DonutGraph;

public class DonutGraphViewModel : ViewModelBase
{
    public ActivitiesHandler ActivitiesData { get; set; }
    
    private ObservableCollection<SliceViewModel> _slices;

    public ObservableCollection<SliceViewModel> Slices
    {
        get => _slices;
        set
        {
            _slices = value;
            OnPropertyChanged();
        }
    }
    
    public int TodayIndex => (int)DateTime.Today.DayOfWeek;

    private DispatcherTimer _clock;
    
    public DonutGraphViewModel(ActivitiesHandler activitiesHandler)
    {
        ActivitiesData = activitiesHandler;

        BuildSlices();
            
        activitiesHandler.Activities!.CollectionChanged += (s, e) =>
        {
            if (e.OldItems != null)
            {
                foreach (ActivityEntry entry in e.OldItems)
                {
                    var vm = Slices.FirstOrDefault(x => x.EntryData!.Id == entry.Id);
                    if (vm == null) continue;
                    Slices.Remove(vm);
                    BuildSlices();
                }
            }
            
            if (e.NewItems != null)
            {
                foreach (ActivityEntry entry in e.NewItems)
                {
                    if (!entry.RepeatOn!.Contains(DateTime.Today.DayOfWeek)) continue; 
                    AddSlice(entry);
                    BuildSlices();
                }
            }
            
        };

        WeakReferenceMessenger.Default.Register<RebuildSlicesMessage> (this, (r, m) => 
            { BuildSlices();});
        
        StartClock();
    }

    public void PopulateActivityInfo(object id)
    {
        var entry = ActivitiesData.Activities!.FirstOrDefault(x => x.Id == (string)id);
        if (entry == null) return;

        var todayIndex = (int)DateTime.Now.DayOfWeek;

        (ClockHour, ClockMinute, ClockMeridiem) = FromAngleToHour(entry.StartAngle[todayIndex]);

        RingColor = ActivityColors.Categories[entry.Color.GroupIndex][entry.Color.ColorIndex];
        UpperInfo = "Activity starts at:";
        LowerInfo = entry.Title;
        ShowMarkActivityAsDoneButton = !entry.Done;
    }

    public void CleanActivityInfo()
    {
        ShowActivityInfo = false;
        ShowMarkActivityAsDoneButton = false;
        LowerInfo = "";
        
        ClockHour = DateTime.Now.ToString("hh");
        ClockMinute = DateTime.Now.ToString("mm");
        ClockMeridiem = DateTime.Now.ToString("tt");
        UpperInfo = $"{ DateTime.Now:yyyy-MM-dd} {DateTime.Now.DayOfWeek} ";
    }
    private void StartClock()
    {
        _clock = new DispatcherTimer();
        _clock.Interval = TimeSpan.FromMinutes(1);
        _clock.Tick += (s, e) => UpdateClock();
        _clock.Start();
        UpdateClock();
    }

    private void UpdateClock()
    {
        ClockHour = DateTime.Now.ToString("hh");
        ClockMinute = DateTime.Now.ToString("mm");
        ClockMeridiem = DateTime.Now.ToString("tt");
        UpperInfo = $"{ DateTime.Now:yyyy-MM-dd} {DateTime.Now.DayOfWeek} ";
        UpdateClockHand();
    }

    private void UpdateClockHand()
    {
        var hour = DateTime.Now.Hour;
        var minute = DateTime.Now.Minute;

        var angle = 270 - (15 * hour + 0.25 * minute);

        angle = angle % 360;
        if (angle < 0) angle += 360;

        var radian = angle * (Math.PI / 180);

        var x1 = 510 + 215 * Math.Cos(radian);
        var y1 = 369 + 215 * Math.Sin(radian);

        var x2 = 510 + 325 * Math.Cos(radian);
        var y2 = 369 + 325 * Math.Sin(radian);

        X1 = Math.Truncate(x1);
        Y1 = Math.Truncate(Math.Abs(y1 - 738));
        X2 = Math.Truncate(x2);
        Y2 = Math.Truncate(Math.Abs(y2 - 738));
        
    }

    private void BuildSlices()
    {
        Slices = new ObservableCollection<SliceViewModel>(
            ActivitiesData.Activities!.Where(x => x!.RepeatOn.Contains(DateTime.Today.DayOfWeek))
                .Select(x => new SliceViewModel(x)));
    }
    
    private void AddSlice(ActivityEntry entry)
    {
        var durationSpam = entry!.Duration.Hours + entry.Duration.Minutes / 60.0;
        
        var startAngle = Slices.Count == 0 ? 180 : Slices[^1].EndAngle;
        var endAngle = startAngle - durationSpam * 15;

        for (var i = 0; i <= 6; i++)
        {
            entry.StartAngle[i] = startAngle;
            entry.EndAngle[i] = endAngle;
        }
        
    }


    public void OnMouseDown(object sender, MouseButtonEventArgs e, Canvas donutCanvas)
    {
        var path = sender as Path;
        DragSlice = path.DataContext as SliceViewModel;
        IsDragging = true;

        var mouse = e.GetPosition(donutCanvas);
        var dx = mouse.X - 510;
        var dy = mouse.Y - 369;
        var angle = Math.Atan2(dy, dx) * (180 / Math.PI);
        if (angle < 0) angle += 360;
        LastClockDegree = (angle - 180 + 360) % 360;
    }
    public void OnMouseMove(MouseEventArgs e, Canvas donutCanvas)
    {
        if (!IsDragging || DragSlice == null) return;

        var mouse = e.GetPosition(donutCanvas);
        var dx = mouse.X - 510;
        var dy = mouse.Y - 369;
        var angle = Math.Atan2(dy, dx) * (180 / Math.PI);
        if (angle < 0) angle += 360;
        var clockDegrees = (angle - 180 + 360) % 360;

        var delta = clockDegrees - LastClockDegree;
        if (delta > 180) delta -= 360;
        if (delta < -180) delta += 360;

        DragSlice.StartAngle = (DragSlice.StartAngle - delta + 360) % 360;
        DragSlice.EndAngle = (DragSlice.EndAngle - delta + 360) % 360;

        LastClockDegree = clockDegrees;

        (ClockHour, ClockMinute, ClockMeridiem) = FromAngleToHour(DragSlice.StartAngle);
        
        UpperInfo = "Starts at:";
    }
    public void OnMouseUp()
    {
        if (DragSlice == null) return; 
            
        var activity = DragSlice.EntryData;
        
        activity.StartAngle[TodayIndex] = DragSlice.StartAngle;

        ActivitiesData.Serialize();
        
        IsOverlapping = HasOverlap(DragSlice);
        
        IsDragging = false;
        DragSlice = null;
        
        ClockHour = DateTime.Now.ToString("hh");
        ClockMinute = DateTime.Now.ToString("mm");
        ClockMeridiem = DateTime.Now.ToString("tt");
        UpperInfo = $"{ DateTime.Now:yyyy-MM-dd} {DateTime.Now.DayOfWeek} ";
        
    }

    public static (string, string, string) FromAngleToHour(double angle)
    {
        var adjustedAngle = (180 - angle + 360) % 360;
        var totalHours = adjustedAngle / 15.0;
        var hour = (int)totalHours + 6;
        hour = hour % 24;
        var minutes = (int)((totalHours % 1) * 60);

        var meridiem = hour >= 12 ? "PM" : "AM";
        var correctHour = hour > 12 ? hour - 12 : hour;
        if (hour == 0) correctHour = 12;
        
        return (correctHour.ToString("D2"), minutes.ToString("D2"), meridiem);
    }
    
    private bool HasOverlap(SliceViewModel dragged)
    {
        foreach (var slice in Slices)
        {
            if (slice == dragged) continue;
            
            if (dragged.StartAngle > slice.EndAngle && dragged.StartAngle < slice.StartAngle) return true;
            if (dragged.EndAngle > slice.EndAngle && dragged.EndAngle < slice.StartAngle) return true;
        }
        return false;
        
    }
    /*Full Properties*/

    private string _lowerInfo;

    public string LowerInfo
    {
        get => _lowerInfo;
        set
        {
            _lowerInfo = value;
            OnPropertyChanged();
        }
    }

    private bool _showActivityInfo;

    public bool ShowActivityInfo
    {
        get => _showActivityInfo;
        set
        {
            _showActivityInfo = value;
            OnPropertyChanged();
        }
    }

    private Brush? _ringColor;

    public Brush? RingColor
    {
        get => _ringColor;
        set
        {
            _ringColor = value;
            OnPropertyChanged();
        }
    }

    private bool _showMarkActivityAsDoneButton;

    public bool ShowMarkActivityAsDoneButton
    {
        get => _showMarkActivityAsDoneButton;
        set
        {
            _showMarkActivityAsDoneButton = value;
            OnPropertyChanged();
        }
    }

    private string _upperInfo;

    public string UpperInfo
    {
        get => _upperInfo;
        set
        {
            _upperInfo = value;
            OnPropertyChanged();
        }
    }

    private double _x1;

    public double X1
    {
        get => _x1;
        set
        {
            _x1 = value;
            OnPropertyChanged();
        }
    }

    private double _y1;

    public double Y1
    {
        get => _y1;
        set
        {
            _y1 = value;
            OnPropertyChanged();
        }
    }

    private double _x2;

    public double X2
    {
        get => _x2;
        set
        {
            _x2 = value;
            OnPropertyChanged();
        }
    }

    private double _y2;

    public double Y2
    {
        get => _y2;
        set
        {
            _y2 = value;
            OnPropertyChanged();
        }
    }

    
    private string _clockHour;

    public string ClockHour
    {
        get => _clockHour;
        set
        {
            _clockHour = value;
            OnPropertyChanged();
        }
    }

    private string _clockMinute;

    public string ClockMinute
    {
        get => _clockMinute;
        set
        {
            _clockMinute = value;
            OnPropertyChanged();
        }
    }

    private string _clockMeridiem;

    public string ClockMeridiem
    {
        get => _clockMeridiem;
        set
        {
            _clockMeridiem = value;
            OnPropertyChanged();
        }
    }

    private bool _isOverlapping;

    public bool IsOverlapping
    {
        get => _isOverlapping;
        set
        {
            _isOverlapping = value;
            OnPropertyChanged();
        }
    }

    private double _lastClockDegree;

    public double LastClockDegree
    {
        get => _lastClockDegree;
        set
        {
            _lastClockDegree = value;
            OnPropertyChanged();
        }
    }

    private SliceViewModel? _dragSlice;

    public SliceViewModel? DragSlice
    {
        get => _dragSlice;
        set
        {
            _dragSlice = value;
            OnPropertyChanged();
        }
    }

    private bool _isDragging;

    public bool IsDragging
    {
        get => _isDragging;
        set
        {
            _isDragging = value;
            OnPropertyChanged();
        }
    }
    
}