using System.DirectoryServices.ActiveDirectory;
using System.Windows;

namespace Vertex.Data.Services;

public static class AngleConverters
{
    public static Point AngleToPoint(double angleDegree, double radius )
    {
        var radians = angleDegree * Math.PI / 180;
        var x = radius * Math.Cos(radians);
        var y = -radius * Math.Sin(radians);
        return new Point(x, y);
    }
    
    public static (int hour, int minute, string meridiem) AngleToHour(double angle)
    {
        var hour24 = ((270 - angle) / 15) % 24;
        if (hour24 < 0) hour24 += 24;
        
        var totalMinutes = (int)Math.Round(hour24 * 60) % 1440;

        var h = totalMinutes / 60;
        var minute = totalMinutes % 60;

        var meridiem = h < 12 ? "AM" : "PM";
        var hour = h % 12 == 0 ? 12 : h % 12;

        return (hour, minute,  meridiem);
    }
    
    public static double HourToAngle(int hour, int minute, string meridiem)
    {
        double hour24 = hour % 12 + (meridiem == "PM" ? 12 : 0);
        hour24 += minute / 60.0;
        var degree = 270 - 15 * hour24;
        
        return (degree % 360 + 360) % 360;
        
    }
}