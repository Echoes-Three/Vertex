using System.Windows;
using System.Windows.Media;
using Vertex.Data.Services;
using Vertex.Models.Entities;
using Vertex.MVVM;
using Colors = Vertex.Data.Services.Colors;

namespace Vertex.ViewModels.DonutGraph;

public class SliceViewModel : ViewModelBase
{
    public ActivityEntry? EntryData { get; }
    
    private const double Radius = 245;
    
    public SliceViewModel(ActivityEntry entry)
    {
        EntryData = entry;
        InitializeSlice();
    }

    private void InitializeSlice()
    {
        var today = (int)DateTime.Today.DayOfWeek;
        var durationSpan = EntryData!.Duration.Hours + EntryData.Duration.Minutes / 60.0;
        
        StartAngle = EntryData.StartAngle[today];
        EndAngle = EntryData.EndAngle[today] = EntryData.StartAngle[today] - durationSpan * 15;
        
        SliceColor = Colors.Palette[EntryData.Color];
    }
    private static Point GetPointOnCircle(double clockAngle)
    {
        var radians = clockAngle * Math.PI / 180;
        var x = Radius * Math.Cos(radians);
        var y = -Radius * Math.Sin(radians);
        return new Point(x, y);
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
    public double StartAngle
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PathData)); // add this
        }
    }
    public double EndAngle
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PathData)); // add this
        }
    }
    public Brush? SliceColor
    {
        get;
        set
        {
            field = value;
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
    
}