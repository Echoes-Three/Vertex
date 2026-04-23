using System.Windows.Media;
using Vertex.Models.EnumDefinitions;

namespace Vertex.Models.UserData.Entry;

public class ActivityEntry(
    string id,
    Color color,
    string name,
    bool completed,
    TimeSpan durationHours,
    int placementOrder)
{
    public string Id { get; set; } = id;
    public Color Color {get; set;} = color;
    public string Name { get; set; } = name;
    public bool Completed { get; set; } = completed;
    public TimeSpan DurationHours { get; set; } = durationHours;
  
    public int PlacementOrder { get; set; } = placementOrder;
    
}