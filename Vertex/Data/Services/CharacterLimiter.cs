namespace Vertex.Data.Services;

public static class CharacterLimiter
{
    public static string LimitActivityTitle(string title) => 
        title.Length >= 30 ? title[..30] :  title;
    
    public static string LimitActivityContent(string body) =>
    body.Length >= 500 ? body[..500] :  body;
    
    public static string LimitReminderContent(string body) => 
        body.Length >= 250 ? body[..250] :  body;
}