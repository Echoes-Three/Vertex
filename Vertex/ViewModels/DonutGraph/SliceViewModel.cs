using System.Windows;
using System.Windows.Media;
using Vertex.Data.Services;
using Vertex.Models.Entities;
using Vertex.MVVM;
using Colors = Vertex.Data.Services.Colors;

namespace Vertex.ViewModels.DonutGraph;

public class SliceViewModel : ViewModelBase
{
    public ActivityEntry? Data { get; }
    
    private const double Radius = 245;
    
    public SliceViewModel(ActivityEntry entry)
    {
        Data = entry;
        InitializeSlice();
    }

    private void InitializeSlice()
    {
        var today = (int)DateTime.Today.DayOfWeek;
        var durationSpan = Data!.Duration.Hours + Data.Duration.Minutes / 60.0;
        
        StartAngle = Data.StartAngle[today];
        EndAngle = Data.EndAngle[today] = Data.StartAngle[today] - durationSpan * 15;
        
        SliceColor = Colors.Palette[Data.Color];
    }
    
    public Geometry PathData
    {
        get
        {
            var p1 = AngleConverters.AngleToPoint(StartAngle, Radius);
            var p2 = AngleConverters.AngleToPoint(EndAngle, Radius);
            
            var figure = new PathFigure { StartPoint = p1, IsClosed = false };
            figure.Segments.Add(new ArcSegment
            {
                Point = p2,
                Size = new Size(Radius, Radius),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = false
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
            OnPropertyChanged(nameof(PathData));
        }
    }
    public double EndAngle
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(PathData));
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
}