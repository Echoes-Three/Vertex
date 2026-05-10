using CommunityToolkit.Mvvm.ComponentModel;

namespace Vertex.Models.Entities.Entry;

public class ReminderEntry : ObservableObject
{
    public string Id { get; set; } = "";
    public string Content { get; set; } = "";
    public bool Done { get; set; } = false;
    public DateTime Setfor { get; set; } = default;
    public DateTime CreatedAt { get; set; } = default;
    public DateTime DonedAt { get; set; } = default;
    
}