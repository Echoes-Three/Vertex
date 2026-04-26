namespace Vertex.Models.UserDataHandling;

public class BreakEntry(
    string id = "",
    TimeSpan durationHour = default,
    int placementOrder = 0)
{
    public string Id { get; set; } = id;
    public TimeSpan DurationHour { get; set;} = durationHour;
    public int PlacementOrder { get; set;} = placementOrder;
}
