namespace Vertex.Models;

public class WeeklySnapshot
{
    public DateTime WeekStart { get; set; }
    public List<DailyData> Days { get; set; } = new();
}