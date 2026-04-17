namespace Vertex.Models.Entities.Entry;

public class SleepEntry
{
    public DateTime SleepTime { get; set; }
    public DateTime WakeupTime { get; set; }
    public TimeSpan Duration { get; set; }
}