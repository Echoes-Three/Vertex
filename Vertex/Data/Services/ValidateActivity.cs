using Vertex.Data.Handlers;

namespace Vertex.Data.Services;

public static class ValidateActivity
{
    
    public static (bool IsValid, string Message) Title(string title)
    {
        var isValid = !string.IsNullOrWhiteSpace(title);
        var warning = isValid ? "" : "- Title must not be empty.";
        
        return  (isValid, warning);
    }
    
    public static (bool IsValid, string Message) WeekDay(List<bool> daysOfWeek)
    {
        var isValid = daysOfWeek.Contains(true);
        var warning = isValid ? "" : "- Must pick at least one day.";
        
        return  (isValid, warning);
    }

    public static (bool IsValid, string Message) Duration(ActivitiesHandler instance, List<DayOfWeek>? daysOfWeek,
        (int Hour, int Minute) duration, string id = "")
    {
        //DO NOT INVERT IF
        if (duration is { Hour: 0, Minute: >= 10 } or { Hour: > 0, Minute: >= 0 })
        {
            var span = new TimeSpan(duration.Hour, duration.Minute, 0);
            var isValid = true;
            var warining = "";
        
            foreach (var day in daysOfWeek!)
            {
                var totalDuration = instance.Activities
                    .Where(e => e!.RepeatOn.Contains(day))
                    .Aggregate(TimeSpan.Zero, (acc, e) => acc + e!.Duration);
                
                if (id != "")
                {
                    var exception = instance.Activities
                            .Where(e => e!.RepeatOn.Contains(day) && e.Id == id)
                            .Select(e => e.Duration)
                            .FirstOrDefault();
                    
                    totalDuration -= exception;
                }
                

                if (totalDuration + span <= TimeSpan.FromHours(24)) continue;
                isValid = false;
                warining += $"{ day},";

            }
        
            warining = isValid ? "" : $"- Activity duration is above 24h limit on: {warining[..^1]}.";
        
            return (isValid, warining);
        }
        
        return (false, "- Minimum activity duration is 10min.");
    }
}