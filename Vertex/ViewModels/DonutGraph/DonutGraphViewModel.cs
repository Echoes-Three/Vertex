using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Data.Handlers;
using Vertex.Models.Entities;
using Vertex.MVVM;
using Colors = Vertex.Data.Services.Colors;

namespace Vertex.ViewModels.DonutGraph;

public class DonutGraphViewModel : ViewModelBase
{
    private ActivitiesHandler _activitiesData;
    private readonly RemindersHandler _remindersData;

    public ObservableCollection<SliceViewModel> Slices
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    
    private int TodayIndex => (int)DateTime.Today.DayOfWeek;
    private readonly List<string> _meridiem = ["AM", "PM"];
    private int _currentMeridiemIndex;
    private int _currentHourCount = 06;
    private int _currentMinuteCount = 00;

    private DispatcherTimer _clock;
    
    public DonutGraphViewModel(ActivitiesHandler activitiesHandler, RemindersHandler remindersHandler)
    {
        _activitiesData = activitiesHandler;
        _remindersData = remindersHandler;

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
        ActivityColor = Colors.GetBrush("#e6e6ea");
        
    }
    
    /*Objects Creation & Initialization*/
    private void AddSlice(ActivityEntry entry)
    {
        var durationSpan = entry!.Duration.Hours + entry.Duration.Minutes / 60.0;
        
        var startAngle = Slices.Count == 0 ? 180 : Slices[^1].EndAngle;
        var endAngle = startAngle - durationSpan * 15;

        for (var i = 0; i <= 6; i++)
        {
            entry.StartAngle[i] = startAngle;
            entry.EndAngle[i] = endAngle;
        }
    }
    private void BuildSlices() =>
        Slices = new ObservableCollection<SliceViewModel>(_activitiesData.Activities!
            .Where(s => s!.RepeatOn.Contains(DateTime.Today.DayOfWeek))
            .Select(s => new SliceViewModel(s)));
    
    
    /*Clock;*/
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
        var today = DateTime.Now;

        ClockTime = $"{today:hh:mm tt}";
        ClockDate = $"{today:yyyy-MM-dd}";
        ClockDayOfTheWeek = $"{today.DayOfWeek.ToString().ToUpper()}";
        
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

        var x1 = 350 + 305 * Math.Cos(radian);
        var y1 = 350 + 305 * Math.Sin(radian);

        var x2 = 350 + 325 * Math.Cos(radian);
        var y2 = 350 + 325 * Math.Sin(radian);

        X1 = Math.Truncate(x1);
        Y1 = Math.Truncate(Math.Abs(y1 - 700));
        X2 = Math.Truncate(x2);
        Y2 = Math.Truncate(Math.Abs(y2 - 700));
        
        OnPropertyChanged(nameof(PathData));
    }
    private static Point GetPointOnCircle(double clockAngle)
    {
        var radians = clockAngle * Math.PI / 180;
        var x = 315 * Math.Cos(radians);
        var y = -315 * Math.Sin(radians);
        return new Point(x, y);
    }
    
    public Geometry PathData
    {
        get
        {
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;

            var angle = 270 - (15 * hour + 0.25 * minute);

            angle = angle % 360;
            if (angle < 0) angle += 360;
            
            var p1 = GetPointOnCircle(180);
            var p2 = GetPointOnCircle(angle);
            
            var spanAngle = 180 - angle;
            if (spanAngle < 0) spanAngle += 360;
            
            var figure = new PathFigure { StartPoint = p1, IsClosed = false };
            figure.Segments.Add(new ArcSegment
            {
                Point = p2,
                Size = new Size(315, 315),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = spanAngle > 180
            });

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            return geometry;
        }
    }
    public double X1
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public double Y1
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public double X2
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public double Y2
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public string ClockDayOfTheWeek
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public string ClockDate
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public string ClockTime
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    
    
        
    /*Setting Slice*/
    public void OnRightMouseDown(object sender, MouseButtonEventArgs e, Canvas donutCanvas)
    {
        var path = sender as Path;
        Slice = path.DataContext as SliceViewModel;

        var mouse = e.GetPosition(donutCanvas);
        var dx = mouse.X - 350;
        var dy = mouse.Y - 350;
        var angle = Math.Atan2(dy, dx) * (180 / Math.PI);
        if (angle < 0) angle += 360;
        LastClockDegree = (angle - 180 + 360) % 360;
    }
    public void OnMouseMove(MouseEventArgs e, Canvas donutCanvas)
    {
        if (!IsDragging || Slice == null) return;

        var mouse = e.GetPosition(donutCanvas);
        var dx = mouse.X - 510;
        var dy = mouse.Y - 377;
        var angle = Math.Atan2(dy, dx) * (180 / Math.PI);
        if (angle < 0) angle += 360;
        var clockDegrees = (angle - 180 + 360) % 360;

        var delta = clockDegrees - LastClockDegree;
        if (delta > 180) delta -= 360;
        if (delta < -180) delta += 360;

        Slice.StartAngle = (Slice.StartAngle - delta + 360) % 360;
        Slice.EndAngle = (Slice.EndAngle - delta + 360) % 360;

        LastClockDegree = clockDegrees;
        

        var entry = Slice.EntryData;
        var time = FromAngleToHour(Slice.StartAngle);
        var hour = $"{(int)entry!.Duration.TotalHours}H";
        var minute = entry.Duration.Minutes == 0
            ? ""
            : $"{entry.Duration.Minutes}MIN";
        var title = entry.Title.Length > 16 ? entry.Title[..10] : entry.Title;
            
        ActivityColor = Colors.Palette[entry.Color];
        ClockTime = $"{time.Hour}:{time.Minute} {time.Meridiem}";
        ClockDayOfTheWeek = $"{title}... -> {hour}{minute}";
        ClockDate = "↑↑↑ STARTS ↑↑↑";
    }
    public void OnMouseUp()
    {
        if (Slice == null) return; 
            
        var activity = Slice.EntryData;
        
        activity!.StartAngle[TodayIndex] = Slice.StartAngle;
        
        _activitiesData.Serialize();
        
        CleanActivityInfo();
        
        IsDragging = false;
        Slice = null;
        BuildSlices();
    }
    private static (string Hour, string Minute, string Meridiem) FromAngleToHour(double angle)
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
    public void PopulateActivityInfo(object pathTag)
    {
        var entry = _activitiesData.Activities!.FirstOrDefault(x => x.Id == (string)pathTag);
        if (entry == null) return;
        
        var today = (int)DateTime.Today.DayOfWeek;
        var time = FromAngleToHour(entry.StartAngle[today]);
        var hour = (int)entry!.Duration.TotalHours == 0
            ? ""
            : $"{(int)entry!.Duration.TotalHours}H";
        var minute = entry.Duration.Minutes == 0
            ? ""
            : $"{entry.Duration.Minutes}MIN";
        var title = entry.Title.Length > 16 ? entry.Title[..10] : entry.Title;
            
        ActivityColor = Colors.Palette[entry.Color];
        ClockTime = $"{time.Hour}:{time.Minute} {time.Meridiem}";
        ClockDayOfTheWeek = $"{title}... -> {hour}{minute}";
        ClockDate = "↑↑↑ STARTS ↑↑↑";
    }
    public void CleanActivityInfo()
    {
        UpdateClock();
        ActivityColor = Colors.GetBrush("#e6e6ea");
    }
    
    private double LastClockDegree
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    private SliceViewModel? Slice
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    private bool IsDragging
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public Brush? ActivityColor
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
}