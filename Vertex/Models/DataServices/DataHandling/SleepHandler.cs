using System.IO;
using System.Text.Json;
using Vertex.Models.Entities.Entry;
using Vertex.Models.Interfaces;

namespace Vertex.Models.DataServices.DataHandling;

public class SleepHandler : IFileHandler
{
    private readonly string _fullPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Vertex", "Data", "SleepSchedule.json");
    public SleepEntry? SleepSchedule { get; set; }

    public void Save(SleepEntry sleepSchedule)
    {
        SleepSchedule = sleepSchedule;
        
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