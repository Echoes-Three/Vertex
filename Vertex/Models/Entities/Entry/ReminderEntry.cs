namespace Vertex.Models.UserData.Entry;

public class ReminderEntry(
    string id,
    string title,
    string content,
    bool completed,
    DateTime createdAt
    )
{
    public string Id { get; set; } = id;
    public string Title { get; set; } = title;
    public string Content { get; set; } = content;
    public bool Completed { get; set; } = completed;
    public DateTime CreatedAt { get; set; } = createdAt;
    
}