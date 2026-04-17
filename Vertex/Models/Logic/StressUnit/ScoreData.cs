using Vertex.Models.UserData.DataHandling;

namespace Vertex.Models.StressUnit;

public class ScoreData
{
    public List<(string Name, double Score)> Scores { get; set; }

    public void GetScores(ActivitiesHandler activitiesHandler)
    {
        Scores = activitiesHandler.Activities.Select(activityEntry => (
            activityEntry.Name,
            Stress.ToScore(
                activityEntry.Energy,
                activityEntry.Enjoyment,
                activityEntry.DurationHours)
        )).ToList();
    }
}