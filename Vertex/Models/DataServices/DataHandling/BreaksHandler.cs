using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Vertex.Models.Interfaces;
using Vertex.Models.UserDataHandling;

namespace Vertex.Models.UserData.DataHandling;

public class BreaksHandler : IFileHandler
{
    public ObservableCollection<BreakEntry>? Breaks { get; set; }
    
    public void Save(BreakEntry entry)
    {
        Breaks!.Add(entry);
        
        var json = JsonSerializer.Serialize(Breaks);

        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex", "Data", "Breaks.json"
        );
        
        File.WriteAllText(fullPath, json);
        
    }

    public void Load()
    {
        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex", "Data", "Breaks.json"
        );
        
        var file = File.ReadAllText(fullPath);
        
        var breaks = JsonSerializer.Deserialize<ObservableCollection<BreakEntry>>(file);

        if (breaks == null) return;
        
        Breaks = breaks;
    }
    
}