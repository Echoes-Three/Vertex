using System.Windows.Media;

namespace Vertex.Models.Entities.Entry;

public class ActivityEntry
{
    public string Id { get; set; } = "";
    public String Color {get; set;} = null;
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public bool Done { get; set; } = false;
    public List<DayOfWeek>? RepeatOn { get; set; } = null;
    public TimeSpan DurationHours { get; set; } = TimeSpan.Zero;
    public int PlacementOrder { get; set; } = 0;
    
}