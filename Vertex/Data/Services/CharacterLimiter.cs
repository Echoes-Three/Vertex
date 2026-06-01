namespace Vertex.Data.Services;

public static class CharacterLimiter
{
    public static string LimitActivityTitle(ref string title) => 
        title.Length >= 25 ? title[..25] :  title;
    
    public static string LimitActivityContent(ref string body) =>
        body.Length >= 500 ? body[..500] :  body;
    
    public static string LimitReminderContent(ref string body) => 
        body.Length >= 250 ? body[..250] :  body;
}