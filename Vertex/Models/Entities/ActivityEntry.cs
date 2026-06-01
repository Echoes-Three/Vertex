
namespace Vertex.Models.Entities;

public class ActivityEntry
{
    public string Id { get; set; } = "";
    public int Color { get; set; } = 0;
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public List<DayOfWeek>? RepeatOn { get; set; } = null;
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public List<double> StartAngle { get; set; } = [0,0,0,0,0,0,0];
    public List<double> EndAngle { get; set; } = [0,0,0,0,0,0,0];
}