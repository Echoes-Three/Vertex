using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Vertex.Models.Entities;

public static class ActivityColors
{
    public static readonly List<List<Brush>> Categories = 
    [
        [
            GetBrush("#FFCC99"),
            GetBrush("#FFB366"),
            GetBrush("#FF9944"),
            GetBrush("#E07020"),
            GetBrush("#B05010"),
            GetBrush("#7A3308")
        ],
        [
            GetBrush("#AAECC0"),
            GetBrush("#77DD99"),
            GetBrush("#55CC7A"),
            GetBrush("#28A050"),
            GetBrush("#186830"),
            GetBrush("#0A3D1A")
        ],
        [
            GetBrush("#FDB9FE"), 
            GetBrush("#FB8AFC"), 
            GetBrush("#F066F0"), 
            GetBrush("#C832C8"), 
            GetBrush("#881F88"),
            GetBrush("#4A0D4A")
        ],
        [
            GetBrush("#99CAFE"),
            GetBrush("#66AAFC"),
            GetBrush("#4490F0"),
            GetBrush("#1A60C8"),
            GetBrush("#0E3D88"),
            GetBrush("#071E4A")
        ],
        [
            GetBrush("#C9AAF0"),
            GetBrush("#AA77E4"),
            GetBrush("#8F55D6"),
            GetBrush("#6228B0"),
            GetBrush("#3E1278"),
            GetBrush("#1E0640")
        ],
        [
            GetBrush("#EBA8C4"),
            GetBrush("#D97AA0"),
            GetBrush("#C8527A"),
            GetBrush("#9E2050"),
            GetBrush("#6B0E30"),
            GetBrush("#3A0418")
        ]
    ];
    public static SolidColorBrush GetBrush(string hex) => (SolidColorBrush)new BrushConverter().ConvertFromString(hex)!;
}