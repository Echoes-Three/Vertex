using Vertex.Models.UserData.DataHandling;

namespace Vertex.Models.ConsistencyUnit;

public class Consistency
{
    public int ToPercentage(ActivitiesHandler  activitiesHandler)
    {
        var activitiesCount = activitiesHandler.Activities.Count;
        var completedActivies = activitiesHandler.Activities.Count(activityEntry => activityEntry.Completed);

        var percentage = (completedActivies / activitiesCount) * 100;
        
        return percentage;
    }
}