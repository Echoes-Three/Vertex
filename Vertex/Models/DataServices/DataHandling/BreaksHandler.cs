using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Vertex.Models.Interfaces;
using Vertex.Models.UserDataHandling;

namespace Vertex.Models.DataServices.DataHandling;

public class BreaksHandler : IFileHandler
{
    private readonly string _fullPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Vertex", "Data", "Breaks.json");
        
    public ObservableCollection<BreakEntry>? Breaks { get; set; }
    
    public void Save(BreakEntry entry)
    {
        Breaks!.Add(entry);
        
        var json = JsonSerializer.Serialize(this, new  JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        File.WriteAllText(_fullPath, json);
        
    }

    public void Load()
    {
        var file = File.ReadAllText(_fullPath);
        
        var handler = JsonSerializer.Deserialize<BreaksHandler>(file);

        if (handler == null) return;
        
        Breaks = handler.Breaks;
    }
    
}