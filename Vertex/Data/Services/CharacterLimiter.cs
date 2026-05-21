namespace Vertex.Data.Services;

public static class CharacterLimiter
{
    public static string LimitActivityTitle(string activityTitle) => 
        activityTitle.Length >= 29 ? activityTitle[..29] :  activityTitle;
    
    public static string LimitContentBody(string body) =>
    body.Length >= 500 ? body[..500] :  body;
}