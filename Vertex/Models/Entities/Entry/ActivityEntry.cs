
namespace Vertex.Models.Entities.Entry;

public class ActivityEntry
{
    public string Id { get; set; } = "";
    public (int GroupIndex, int ColorIndex) Color { get; set; } = (0, 0);
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public bool Done { get; set; } = false;
    public List<DayOfWeek>? RepeatOn { get; set; } = null;
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public List<int> Order { get; set; } = [0,0,0,0,0,0,0];
    public List<double> StartAngle { get; set; } = [0,0,0,0,0,0,0];
    public List<double> EndAngle { get; set; } = [0,0,0,0,0,0,0];
}