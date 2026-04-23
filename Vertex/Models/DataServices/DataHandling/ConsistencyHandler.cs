using System.IO;
using System.Text.Json;
using Vertex.Models.Entities.Entry;
using Vertex.Models.Interfaces;

namespace Vertex.Models.DataServices.DataHandling;

public class ConsistencyHandler: IFileHandler
{
    public ConsistencyEntry? Consistency { get; set; }
    
    public void Save(int percentage)
    {
        
        if (Consistency!.CurrentWeek.Count == 7)
        {
            Consistency.LastWeek = Consistency.CurrentWeek;
            Consistency.CurrentWeek.Clear();
        }
        
        Consistency.CurrentWeek.Add(percentage);
        
        var json = JsonSerializer.Serialize(Consistency);

        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex", "Data", "Consistency.json"
        );
        
        File.WriteAllText(fullPath, json);
    }

    public void Load()
    {
        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex", "Data", "Consistency.json"
        );
        
        var file = File.ReadAllText(fullPath);
        
        var consistency = JsonSerializer.Deserialize<ConsistencyEntry>(file);
        
        if (consistency == null) return;
        
        Consistency.CurrentWeek = consistency.CurrentWeek;
        Consistency.LastWeek = consistency.LastWeek;
        
    }
}