using Vertex.Models.DataServices.DataHandling;
using Vertex.Models.UserData.DataHandling;

namespace Vertex.Models.ConsistencyUnit;

public static class Consistency
{
    public static int ToPercentage(this ActivitiesHandler  activitiesHandler)
    {
        var activitiesCount = activitiesHandler.Activities.Count;
        var completedActivies = activitiesHandler.Activities.Count(activityEntry => activityEntry.Completed);

        var percentage = (completedActivies / activitiesCount) * 100;
        
        return percentage;
    }
}