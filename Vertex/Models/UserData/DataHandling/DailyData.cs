using System.IO;
using System.Text.Json;
using Vertex.Models.Interfaces;
using Vertex.Models.UserData.Entry;

namespace Vertex.Models.UserData.DataHandling;

public class DailyData : IFileHandler
{
    public List<ActivityEntry> Activities { get; set; } = new();
    
    // Loasds activities list to JSON file
    public void Load(ActivityEntry entry)
    {
        Activities.Add(entry);
        
        var json = JsonSerializer.Serialize(Activities);

        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex"
        );
        
        var dataPath = Path.Combine(fullPath, "Data");
        var dailyPath = Path.Combine(dataPath, "Daily");

        Directory.CreateDirectory(dailyPath);
        
        var filePath = Path.Combine(dailyPath, "DailyData.json");
        
        File.WriteAllText(filePath, json);
    }
    
    // Pulls the data from the JSON file
    public ActivityEntry Unload()
    {
        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex"
        );
        
        var dataPath = Path.Combine(fullPath, "Data");
        var dailyPath = Path.Combine(dataPath, "Daily");
        var filePath = Path.Combine(dailyPath, "DailyData.json");
        
        var jsonFile = File.ReadAllText(filePath);
        
        ActivityEntry activities = JsonSerializer.Deserialize<ActivityEntry>(jsonFile);
        
        return activities;
    }
    
    // Sends the data to the weekly JSON file
    public void Snapshot()
    {
        
    }
    
}