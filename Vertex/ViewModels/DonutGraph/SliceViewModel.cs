using System.Windows;
using System.Windows.Media;
using Vertex.Models.Entities;
using Vertex.Models.Entities.Entry;
using Vertex.MVVM;

namespace Vertex.ViewModels.DonutGraph;

public class SliceViewModel : ViewModelBase
{
    public ActivityEntry? EntryData { get; }
    public DonutGraphViewModel Vm { get; }
    
    private const double Radius = 245;
    
    public SliceViewModel(ActivityEntry entry)
    {
        EntryData = entry;
        InitializeSlice();
    }

    private void InitializeSlice()
    {
        var today = (int)DateTime.Today.DayOfWeek;
        var durationSpam = EntryData!.Duration.Hours + EntryData.Duration.Minutes / 60.0;
        
        StartAngle = EntryData.StartAngle[today];
        EndAngle = EntryData.EndAngle[today] = EntryData.StartAngle[today] - durationSpam * 15;
        
        SliceColor = ActivityColors.Categories[EntryData.Color.GroupIndex][EntryData.Color.ColorIndex];
    }
    
    private int _sliceOrder;

    public int SliceOrder
    {
        get => _sliceOrder;
        set
        {
            _sliceOrder = value;
            OnPropertyChanged();
        }
    }

    
    private double _startAngle;

    public double StartAngle
    {
        get => _startAngle;
        set
        {
            _startAngle = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PathData)); // add this
        }
    }

    private double _endAngle;

    public double EndAngle
    {
        get => _endAngle;
        set
        {
            _endAngle = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PathData)); // add this
        }
    }

    private  Brush? _sliceColor;

    public  Brush? SliceColor
    {
        get => _sliceColor;
        set
        {
            _sliceColor = value;
            OnPropertyChanged();
        }
    }

    public double SpanAngle
    {
        get
        {
            var span = StartAngle - EndAngle;
            if (span < 0) span += 360;
            return span;
        }
    }
    
    public Geometry PathData
    {
        get
        {
            var p1 = GetPointOnCircle(StartAngle);
            var p2 = GetPointOnCircle(EndAngle);
            
            var figure = new PathFigure { StartPoint = p1, IsClosed = false };
            figure.Segments.Add(new ArcSegment
            {
                Point = p2,
                Size = new Size(Radius, Radius),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = SpanAngle > 180
            });

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            return geometry;
        }
    }
    
    private static Point GetPointOnCircle(double clockAngle)
    {
        var radians = clockAngle * Math.PI / 180;
        var x = Radius * Math.Cos(radians);
        var y = -Radius * Math.Sin(radians);
        return new Point(x, y);
    }
    
}