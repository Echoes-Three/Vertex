namespace Vertex.Data.Services;

public static class ActivityRepeat
{
    public struct DaysOfWeekBool
    {
        public bool Sun;
        public bool Mon;
        public bool Tue;
        public bool Wed;
        public bool Thu;
        public bool Fri;
        public bool Sat;
    }
    
    public static List<DayOfWeek> ToDayOfWeek(this List<bool> daysOfWeek)
    {
        var converted = new List<DayOfWeek>();
        
        for (var index = 0; index <= 6; index++)
        {
            if (daysOfWeek[index]) converted.Add((DayOfWeek)index);
        }
        return converted;
    }
    
    public static DaysOfWeekBool ToBoolList(this List<DayOfWeek>? daysOfWeek)
    {
        return new DaysOfWeekBool
        {
            Sun = daysOfWeek?.Contains(DayOfWeek.Sunday) ?? false,
            Mon = daysOfWeek?.Contains(DayOfWeek.Monday) ?? false,
            Tue = daysOfWeek?.Contains(DayOfWeek.Tuesday) ?? false,
            Wed = daysOfWeek?.Contains(DayOfWeek.Wednesday) ?? false,
            Thu = daysOfWeek?.Contains(DayOfWeek.Thursday) ?? false,
            Fri = daysOfWeek?.Contains(DayOfWeek.Friday) ?? false,
            Sat = daysOfWeek?.Contains(DayOfWeek.Saturday) ?? false,
            
        };
    }
    
}