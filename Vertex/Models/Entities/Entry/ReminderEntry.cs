using System.Text.Json.Serialization;

namespace Vertex.Models.UserData.Entry;

public class ReminderEntry
{
    public string Id { get; set; } = "";
    public string Content { get; set; } = "";
    public bool Completed { get; set; } = false;
    public DateTime Setfor { get; set; } = default;
    public DateTime CreatedAt { get; set; } = default;
    public DateTime DonedAt { get; set; } = default;
    
}