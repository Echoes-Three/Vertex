using Vertex.Data.Handlers;

namespace Vertex.Data.Services;

public static class ValidateActivity
{
    
    public static (bool IsValid, string Message) Title(string title)
    {
        var isValid = !string.IsNullOrWhiteSpace(title);
        var warning = isValid ? "" : "TITLE MUST NOT BE EMPTY!";
        
        return  (isValid, warning);
    }
    
    public static (bool IsValid, string Message) WeekDay(List<bool> daysOfWeek)
    {
        var isValid = daysOfWeek.Contains(true);
        var warning = isValid ? "" : "MUST PICK AT LEAST ONE DAY!";
        
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
                warining += $" {day.ToString()[..3]},";

            }
        
            warining = isValid ? "" : $"DURATION IS ABOVE 24H LIMIT ON:{warining[..^1]}!";
        
            return (isValid, warining);
        }
        
        return (false, "MINIMUM DURATION IS 10MIN!");
    }
}