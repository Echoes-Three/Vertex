using System.IO;
using System.Text.Json;
using Vertex.Data.Handlers;

namespace Vertex.Data.Services;

public class DataService
{
    private readonly string _dataPath;
    public DataService()
    {
        _dataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex", "Data"
        );
        
        Directory.CreateDirectory(_dataPath);
        
        InitializeFile<ActivitiesHandler>("Activities.json");
        InitializeFile<RemindersHandler>("Reminders.json");
    }

    public void InitializeFile<T>(string fileNeme) where T : new()
    {
        var fullPath = Path.Combine(_dataPath, fileNeme);
        if (File.Exists(fullPath) && new FileInfo(fullPath).Length > 0) return;
        
        var dataStructure =  new T();
        var json = JsonSerializer.Serialize(dataStructure, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
        
        File.WriteAllText(fullPath, json);
    }
    
}