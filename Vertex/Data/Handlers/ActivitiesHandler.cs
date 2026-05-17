using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Vertex.Models.Contracts;
using Vertex.Models.Entities.Entry;
using Vertex.MVVM;

namespace Vertex.Data.Handlers;

public class ActivitiesHandler : ViewModelBase, IFileHandler<ActivityEntry>
{
    private readonly string _fullPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "Vertex", "Data", "Activities.json");
    public ObservableCollection<ActivityEntry> Activities { get; set; }
    
    public void Save(ActivityEntry entry)
    {
        Activities.Add(entry);
        Serialize();
        
    }
    
    public void Delete(ActivityEntry entry)
    {
        Activities!.Remove(entry);
        Serialize();
    }

    public void Serialize()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        });
        
        File.WriteAllText(_fullPath, json);
    }

    public void Load()
    {
        var file = File.ReadAllText(_fullPath);
        
        var options = new JsonSerializerOptions
        {
            IncludeFields = true
        };
        
        var handler = JsonSerializer.Deserialize<ActivitiesHandler>(file, options);

        if (handler == null) return;
        
        Activities = handler.Activities;
    }
}