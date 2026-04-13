using System.IO;
using System.Text.Json;
using Vertex.Models.Interfaces;
using Vertex.Models.UserData.Entry;

namespace Vertex.Models.UserData.DataHandling;

public class ReminderData : IFileHandler
{
    public List<ReminderEntry>  Reminders { get; set; }
    
    public void Load(ReminderEntry entry)
    {
        Reminders.Add(entry);
        
        var json = JsonSerializer.Serialize(Reminders);

        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex"
        );
        
        var dataPath = Path.Combine(fullPath, "Data");
        var remindersPath = Path.Combine(dataPath, "Reminders");

        Directory.CreateDirectory(remindersPath);
        
        var filePath = Path.Combine(remindersPath, "Reminders.json");
        
        File.WriteAllText(remindersPath, json);
    }

    public ReminderEntry Unload()
    {
        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex"
        );
        
        var dataPath = Path.Combine(fullPath, "Data");
        var dailyPath = Path.Combine(dataPath, "Daily");
        var filePath = Path.Combine(dailyPath, "DailyData.json");
        
        var jsonFile = File.ReadAllText(filePath);
        
        ReminderEntry reminders = JsonSerializer.Deserialize<ReminderEntry>(jsonFile);
        
        return reminders;
    }
}