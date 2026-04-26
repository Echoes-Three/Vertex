using System.IO;
using System.Text.Json;
using Vertex.Models.Entities.Entry;
using Vertex.Models.Interfaces;

namespace Vertex.Models.DataServices.DataHandling;

public class ConsistencyHandler: IFileHandler
{
    private readonly string _fullPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Vertex", "Data", "Consistency.json");
    public ConsistencyEntry? Consistency { get; set; }
    
    public void Save(int percentage)
    {
        if (Consistency!.CurrentWeek.Count == 7)
        {
            Consistency.LastWeek = Consistency.CurrentWeek;
            Consistency.CurrentWeek.Clear();
        }
        
        Consistency.CurrentWeek.Add(percentage);
        
        var json = JsonSerializer.Serialize(this, new  JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        File.WriteAllText(_fullPath, json);
    }

    public void Load()
    {
        var file = File.ReadAllText(_fullPath);
        
        var handler = JsonSerializer.Deserialize<ConsistencyHandler>(file);
        
        if (handler == null) return;
        
        Consistency?.CurrentWeek = handler.Consistency?.CurrentWeek;
        Consistency?.LastWeek = handler.Consistency?.LastWeek;
        
    }
}