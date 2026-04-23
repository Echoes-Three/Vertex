using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Media;

namespace Vertex.Models.Entities.ActivityType;

public static class ActivityCategory
{
    public static readonly Dictionary<string, ReadOnlyCollection<Brush>> Categories =
        new Dictionary<string, ReadOnlyCollection<Brush>>
        { 
            { "define", new List<Brush> {
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert")
            }.AsReadOnly() },
            
            { "define", new List<Brush> {
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert")
            }.AsReadOnly() },
            
            { "define", new List<Brush> {
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert")
            }.AsReadOnly() },
            
            { "define", new List<Brush> {
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert"),
                GetBrush("insert")
            }.AsReadOnly() },
            
        };
    
    private static Brush GetBrush(string hex) => (SolidColorBrush)new BrushConverter().ConvertFromString(hex)!;
}