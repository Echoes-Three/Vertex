namespace Vertex.Models.UserData.Entry;

public class ReminderEntry(
    string id,
    string title,
    string content,
    bool completed,
    DateTime createdAt,
    DateTime doneAt,
    DateTime setFor)
{
    public string Id { get; set; } = id;
    public string Title { get; set; } = title;
    public string Content { get; set; } = content;
    public bool Completed { get; set; } = completed;
    public DateTime Setfor { get; set; } = setFor;
    public DateTime CreatedAt { get; set; } = createdAt;
    public DateTime DonedAt { get; set; } = doneAt;
}