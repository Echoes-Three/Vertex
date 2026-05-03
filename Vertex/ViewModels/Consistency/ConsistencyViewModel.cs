using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Vertex.Models.ConsistencyUnit;
using Vertex.Models.DataServices.DataHandling;
using Vertex.Models.Entities.Entry;
using Vertex.MVVM;

namespace Vertex.ViewModels;

public class ConsistencyViewModel : ViewModelBase
{
    private double _canvasWidth;
    public double CanvasWidth
    {
        get => _canvasWidth;
        set
        {
            _canvasWidth = value;
            OnPropertyChanged();
        }
    }

    private double _canvasHeight;
    public double CanvasHeight
    {
        get => _canvasHeight;
        set
        {
            _canvasHeight = value;
            OnPropertyChanged();
        }
    }
    
    private ConsistencyHandler ConsistencyData {get; set;}
    
    public ConsistencyViewModel( ConsistencyHandler consistencyData)
    {
        ConsistencyData = consistencyData;
    }
    
    public void OnCanvasChange()
    {
        var percentageTest = new ConsistencyEntry
        {
            CurrentWeek = [10, 45, 77, 100, 33, 10, 0],
            LastWeek = [22, 70, 66, 90, 0, 0, 10]
        };

        Consistency = (CanvasWidth, CanvasHeight, percentageTest).LoadGraph();
    }
    
    
    private (PointCollection CurrentWeek,PointCollection LastWeek) _consistency;

    public (PointCollection CurrentWeek, PointCollection LastWeek) Consistency
    {
        get => _consistency;
        set
        {
            _consistency = value;
            OnPropertyChanged();
        }
    }

    public PointCollection CurentWeekConsistency => Consistency.CurrentWeek;
    public PointCollection LastWeekConsistency => Consistency.LastWeek;
}