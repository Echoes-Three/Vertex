using System.Windows.Media;
using Vertex.Models.EnumDefinitions;

namespace Vertex.Models.UserData.Entry;

public class ActivityEntry(
    string id = "",
    Color? color = null,
    string name = "",
    bool completed = false,
    TimeSpan durationHours = default,
    int placementOrder = 0)
{
    public string Id { get; set; } = id;
    public Color? Color {get; set;} = color;
    public string Name { get; set; } = name;
    public bool Completed { get; set; } = completed;
    public TimeSpan DurationHours { get; set; } = durationHours;
  
    public int PlacementOrder { get; set; } = placementOrder;
    
}