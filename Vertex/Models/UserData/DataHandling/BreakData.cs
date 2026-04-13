using System.IO;
using System.Text.Json;
using Vertex.Models.Interfaces;
using Vertex.Models.UserDataHandling;

namespace Vertex.Models.UserData.DataHandling;

public class BreakData : IFileHandler
{
    public List<BreakEntry> Breaks { get; set; }
    
    public void Load(BreakEntry entry)
    {
        Breaks.Add(entry);
        
        var json = JsonSerializer.Serialize(Breaks);

        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex"
        );
        
        var dataPath = Path.Combine(fullPath, "Data");
        var breaksPath = Path.Combine(dataPath, "Breaks");

        Directory.CreateDirectory(breaksPath);
        
        var filePath = Path.Combine(breaksPath, "Breaks.json");
        
        File.WriteAllText(filePath, json);
        
    }

    public BreakEntry Unload()
    {
        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex"
        );
        
        var dataPath = Path.Combine(fullPath, "Data");
        var dailyPath = Path.Combine(dataPath, "Daily");
        var filePath = Path.Combine(dailyPath, "DailyData.json");
        
        var jsonFile = File.ReadAllText(filePath);
        
        BreakEntry breaks = JsonSerializer.Deserialize<BreakEntry>(jsonFile);
        
        return breaks;
    }
    
}