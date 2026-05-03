namespace Vertex.Models.UserDataHandling;

public class BreakEntry
{
    public string Id { get; set; } = "";
    public TimeSpan DurationHour { get; set;} = TimeSpan.Zero;
    public int PlacementOrder { get; set;} = 0;
}
