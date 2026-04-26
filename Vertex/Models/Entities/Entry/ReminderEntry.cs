namespace Vertex.Models.UserData.Entry;

public class ReminderEntry(
    string id = "",
    string content = "",
    bool completed = false,
    DateTime createdAt = default,
    DateTime doneAt = default,
    DateTime setFor = default)
{
    public string Id { get; set; } = id;
    public string Content { get; set; } = content;
    public bool Completed { get; set; } = completed;
    public DateTime Setfor { get; set; } = setFor;
    public DateTime CreatedAt { get; set; } = createdAt;
    public DateTime DonedAt { get; set; } = doneAt;
}