using System.IO;
using System.Text.Json;
using Vertex.Models.Contracts;
using Vertex.Models.Entities.Entry;

namespace Vertex.Data.Handlers;

public class SleepHandler : IFileHandler<SleepEntry>
{
    private readonly string _fullPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Vertex", "Data", "SleepSchedule.json");
    public SleepEntry? SleepSchedule { get; set; }

    public void Save(SleepEntry entry)
    {
        SleepSchedule = entry;
        
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented =  true
        });
        
        File.WriteAllText(_fullPath, json);
    }
    
    public void Load()
    {
        var file = File.ReadAllText(_fullPath);
        
        var handler = JsonSerializer.Deserialize<SleepHandler>(file);
        
        if (handler == null) return;
        
        SleepSchedule = handler.SleepSchedule;
    }
}