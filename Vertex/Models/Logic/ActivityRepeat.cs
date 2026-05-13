using Vertex.Models.Entities.Entry;

namespace Vertex.Models.Logic;

public static class ActivityRepeat
{
    public static List<DayOfWeek>? ToDayOfWeek(this List<bool> daysOfWeek)
    {
        var converted = new List<DayOfWeek>();
        
        for (var index = 0; index <= 6; index++)
        {
            if (daysOfWeek[index]) converted.Add((DayOfWeek)index + 1);
        }
        return converted;
    }
    
    public static bool HasMinimumValue(List<bool> daysOfWeek) => daysOfWeek.Contains(true);
    
}