using System.IO;
using System.Text.Json;
using Vertex.Models.Entities.Entry;
using Vertex.Models.Interfaces;

namespace Vertex.Models.DataServices.DataHandling;

public class SleepHandler : IFileHandler
{
    public SleepEntry? SleepSchedule { get; set; }

    public void Save( SleepEntry sleepSchedule)
    {
        SleepSchedule = sleepSchedule;
        
        var json = JsonSerializer.Serialize(SleepSchedule);

        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex", "Data", "Sleep.json"
        );
        
        File.WriteAllText(fullPath, json);
    }
    public void Load()
    {
        var json = JsonSerializer.Serialize(SleepSchedule);

        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex", "Data", "Sleep.json"
        );
        
        
        var file = File.ReadAllText(fullPath);
        
        var sleepSchedule = JsonSerializer.Deserialize<SleepEntry>(file);
        
        if (sleepSchedule == null) return;
        
        SleepSchedule = sleepSchedule;
    }
}