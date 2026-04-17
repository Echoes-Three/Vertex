using System.IO;
using System.Text.Json;
using Vertex.Models.Interfaces;
using Vertex.Models.UserData.Entry;

namespace Vertex.Models.DataServices.DataHandling;

public class RemindersHandler : IFileHandler
{
    public List<ReminderEntry>?  Reminders { get; set; }
    
    public void Save(ReminderEntry entry)
    {
        Reminders!.Add(entry);
        
        var json = JsonSerializer.Serialize(Reminders);

        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex", "Data", "Reminders.json"
        );
        
        File.WriteAllText(fullPath, json);
    }

    public void Load()
    {
        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex", "Data", "Reminders.json"
        );
        
        var file = File.ReadAllText(fullPath);
        
        var reminders = JsonSerializer.Deserialize<List<ReminderEntry>>(file);
        
        if (reminders == null) return;
        
        Reminders = reminders;
    }
}