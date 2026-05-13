using System.Windows;
using System.Windows.Media;
using Vertex.Models.DataServices.DataHandling;
using Vertex.Models.Entities.Entry;

namespace Vertex.Models.ConsistencyUnit;

public static class Consistency
{
    private record struct Points(double Start, double End, double Height);
    public static int ToPercentage(this ActivitiesHandler  activitiesHandler)
    {
        var activitiesCount = activitiesHandler.Activities.Count;
        var completedActivies = activitiesHandler.Activities.Count(activityEntry => activityEntry.Done);

        var percentage = (completedActivies / activitiesCount) * 100;
        
        return percentage;
    }

    public static (PointCollection CurrentWeek, PointCollection LastWeek) LoadGraph(this (double canvasWidth, double canvasHeight, ConsistencyEntry percentage) data)
    {
        var currentWeek = new PointCollection( 
        ConvertToPoints(data.canvasWidth, data.canvasHeight, data.percentage.CurrentWeek).SelectMany(
                points=> new[] { new Point(points.Start,points.Height), new Point(points.End,points.Height)}));
        
        var lastWeek = new PointCollection( 
            ConvertToPoints(data.canvasWidth, data.canvasHeight, data.percentage.LastWeek).SelectMany(
                points=> new[] { new Point(points.Start,points.Height), new Point(points.End,points.Height)}));

        return (currentWeek, lastWeek);
    }

    private static List<Points> ConvertToPoints(double canvasWidth, double canvasHeight, List<int> percentages)
    {
        var piece = canvasWidth / 21;
        var points = new List<Points>();

        for (var (placement, weekDay) = (1,0); weekDay <= 6 ; placement += 3,  weekDay++)
        {
            points.Add(new Points(
                piece * placement,
                piece * placement + piece,
                Math.Abs(canvasHeight / 100 * percentages[weekDay] - canvasHeight)));
        }

        return points;
    }
}