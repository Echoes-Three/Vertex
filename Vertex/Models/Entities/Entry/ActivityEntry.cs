using System.Windows.Media;
using Vertex.Models.EnumDefinitions;

namespace Vertex.Models.UserData.Entry;

public class ActivityEntry
{
    public string Id { get; set; } = "";
    public Color? Color {get; set;} = null;
    public string Name { get; set; } = "";
    public bool Completed { get; set; } = false;
    public TimeSpan DurationHours { get; set; } = TimeSpan.Zero;
    public int PlacementOrder { get; set; } = 0;
    
}