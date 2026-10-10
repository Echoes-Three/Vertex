using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Data.Handlers;
using Vertex.Data.Services;
using Vertex.Models.Entities;
using Vertex.MVVM;
using Colors = Vertex.Data.Services.Colors;

namespace Vertex.ViewModels.DonutGraph;

public class DonutGraphViewModel : ViewModelBase
{
    private ActivitiesHandler _data;
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
    
    private readonly int _today = (int)DateTime.Today.DayOfWeek;
    private readonly List<string> _meridiem = ["AM", "PM"];
    private int _currentMeridiemIndex;
    private int _currentHourCount = 06;
    private int _currentMinuteCount = 00;

    private DispatcherTimer _clock;
    
    public DonutGraphViewModel(ActivitiesHandler activitiesHandler, RemindersHandler remindersHandler)
    {
        _data = activitiesHandler;
        _remindersData = remindersHandler;

        BuildSlices();
            
        activitiesHandler.Activities!.CollectionChanged += (s, e) =>
        {
            if (e.OldItems != null)
            {
                foreach (ActivityEntry entry in e.OldItems)
                {
                    var vm = Slices.FirstOrDefault(x => x.Data!.Id == entry.Id);
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
        Slices = new ObservableCollection<SliceViewModel>(_data.Activities!
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
  
    
    public Geometry PathData
    {
        get
        {
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;

            var angle = 270 - (15 * hour + 0.25 * minute);

            angle %= 360;
            if (angle < 0) angle += 360;
            
            var p1 = AngleConverters.AngleToPoint(180, 315);
            var p2 = AngleConverters.AngleToPoint(angle, 315);
            
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
        var x = mouse.X - 350;
        var y = mouse.Y - 350;
        
        WeakReferenceMessenger.Default.Send(new SetSliceMessage((Slice!.Data.Id, x, y)));
    }
    
    private static (string Hour, string Minute, string Meridiem) FromAngleToHour(double angle)
    {
        var adjustedAngle = (180 - angle + 360) % 360;
        var totalHours = adjustedAngle / 15.0;
        var hour = (int)totalHours + 6;
        hour %= 24;
        var minutes = (int)((totalHours % 1) * 60);

        var meridiem = hour >= 12 ? "PM" : "AM";
        var correctHour = hour > 12 ? hour - 12 : hour;
        if (hour == 0) correctHour = 12;
        
        return (correctHour.ToString("D2"), minutes.ToString("D2"), meridiem);
    }
    public void PopulateActivityInfo(object id)
    {
        var entry = _data.Activities!.FirstOrDefault(x => x.Id == (string)id);
        if (entry == null) return;
        
        var time = FromAngleToHour(entry.StartAngle[_today]);
        /*var title = entry.Title.Length > 16 ? entry.Title[..10] : entry.Title;*/
        var title = entry.Title;
            
        ActivityColor = Colors.Palette[entry.Color];
        ClockTime = $"{time.Hour}:{time.Minute} {time.Meridiem}";
        ClockDayOfTheWeek = title;
        ClockDate = "↑↑↑ STARTS AT ↑↑↑";
    }
    public void CleanActivityInfo()
    {
        UpdateClock();
        ActivityColor = Colors.GetBrush("#e6e6ea");
        
        /*_data.Serialize();
        
        CleanActivityInfo();
        
        Slice = null;
        BuildSlices();*/
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