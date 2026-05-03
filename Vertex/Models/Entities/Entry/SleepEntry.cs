namespace Vertex.Models.Entities.Entry;

public class SleepEntry
{
    public DateTime? SleepTime { get; set; } = null;
    public DateTime? WakeupTime { get; set; } = null;
    public TimeSpan? Duration { get; set; } = null;
}