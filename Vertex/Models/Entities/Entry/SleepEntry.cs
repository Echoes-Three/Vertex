namespace Vertex.Models.Entities.Entry;

public class SleepEntry(
    DateTime sleepTime,
    DateTime wakeUpTime,
    TimeSpan duration)
{
    public DateTime SleepTime { get; set; } = sleepTime;
    public DateTime WakeupTime { get; set; } = wakeUpTime;
    public TimeSpan Duration { get; set; } = duration;
}