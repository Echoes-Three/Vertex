namespace Vertex.Data.Services;

public static class CharacterLimiter
{
    public static string LimitActivityTitle(string activityTitle) => 
        activityTitle.Length >= 42 ? activityTitle[..42] :  activityTitle;
}