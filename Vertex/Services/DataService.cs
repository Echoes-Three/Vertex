using System.IO;
using System.Windows.Media;
using Vertex.Models;
using Vertex.Models.EnumDefinitions;
using System.Text.Json;
using Vertex.Models.UserData.DataHandling;


namespace Vertex.Services;

public class DataService
{
    private readonly string _dataPath;
    public DataService()
    {
        var _dataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex", "Data"
        );
        
        Directory.CreateDirectory(_dataPath);
        
        InitializeFile<ActivitiesHandler>("Activities.json");
        InitializeFile<BreaksHandler>("Breaks.json");
        InitializeFile<RemindersHandler>("Reminders.json");
        InitializeFile<ConsistencyHandler>("Weeks.json");
    }

    public void InitializeFile<T>(string fileNeme) where T : new()
    {
        var fullPath = Path.Combine(_dataPath, fileNeme);
        if (File.Exists(fileNeme)) return;
        
        var dataStructure =  new T();
        var json = JsonSerializer.Serialize(dataStructure, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
        
        File.WriteAllText(fullPath, json);
    }
    
}