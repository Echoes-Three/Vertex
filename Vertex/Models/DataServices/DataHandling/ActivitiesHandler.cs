using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Vertex.Models.Entities.Entry;
using Vertex.Models.Interfaces;
using Vertex.MVVM;

namespace Vertex.Models.DataServices.DataHandling;

public class ActivitiesHandler : ViewModelBase, IFileHandler<ActivityEntry>
{
    private readonly string _fullPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "Vertex", "Data", "Activities.json");
    public ObservableCollection<ActivityEntry> Activities { get; set; } = new();
    
    public void Save(ActivityEntry entry)
    {
        Activities.Add(entry);
        
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        File.WriteAllText(_fullPath, json);
    }
    
    public void Load()
    {
        var file = File.ReadAllText(_fullPath);
        
        var handler = JsonSerializer.Deserialize<ActivitiesHandler>(file);

        if (handler == null) return;
        
        Activities = handler.Activities;
    }
}