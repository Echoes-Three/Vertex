namespace Vertex.Models.Entities.Entry;

public class SleepEntry(
    DateTime? sleepTime = null,
    DateTime? wakeUpTime = null,
    TimeSpan? duration = null)
{
    public DateTime? SleepTime { get; set; } = sleepTime;
    public DateTime? WakeupTime { get; set; } = wakeUpTime;
    public TimeSpan? Duration { get; set; } = duration;
}