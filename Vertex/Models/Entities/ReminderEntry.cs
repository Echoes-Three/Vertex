using CommunityToolkit.Mvvm.ComponentModel;

namespace Vertex.Models.Entities;

public class ReminderEntry : ObservableObject
{
    public string Id { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime SetFor { get; set; } = default;
}