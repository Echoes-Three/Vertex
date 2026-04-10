namespace Vertex.Models;

public class DailyData
{
    public DateTime Date { get; set; }
    public List<ActivityEntry> Activities { get; set; } = new();
}