using Vertex.Models.UserData.DataHandling;

namespace Vertex.Models.UserData.Entry;

public class WeeklyEntry
{
    public DateTime WeekStart { get; set; }
    public List<DailyData> Days { get; set; } = new();
    
}