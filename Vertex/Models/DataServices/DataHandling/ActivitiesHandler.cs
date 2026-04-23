using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Vertex.Models.Interfaces;
using Vertex.Models.UserData.Entry;

namespace Vertex.Models.DataServices.DataHandling;

public class ActivitiesHandler : IFileHandler
{
    public ObservableCollection<ActivityEntry> Activities { get; set; } = new();
    
    public void Save(ActivityEntry entry)
    {
        Activities.Add(entry);
        
        var json = JsonSerializer.Serialize(Activities);

        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex", "Data", "Activities.json"
        );
        
        File.WriteAllText(fullPath, json);
    }
    
    public void Load()
    {
        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex", "Data", "Activities.json"
        );
        var file = File.ReadAllText(fullPath);
        
        var activities = JsonSerializer.Deserialize<ObservableCollection<ActivityEntry>>(file);

        if (activities == null) return;
        
        Activities = activities;
    }
    
}