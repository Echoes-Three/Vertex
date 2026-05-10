using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Vertex.Models.Entities.Entry;
using Vertex.Models.Interfaces;

namespace Vertex.Models.DataServices.DataHandling;

public class RemindersHandler : IFileHandler<ReminderEntry>
{
    private readonly string _fullPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Vertex", "Data", "Reminders.json");
    public ObservableCollection<ReminderEntry>? Reminders { get; set; }
    
    public void Save(ReminderEntry entry)
    {
        Reminders!.Add(entry);
        Serialize();
    }

    public void Delete(ReminderEntry entry)
    {
        Reminders!.Remove(entry);
        Serialize();
    }

    public void Serialize()
    {
        var json = JsonSerializer.Serialize(this, new  JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        File.WriteAllText(_fullPath, json);
    }

    public void Load()
    {
        var file = File.ReadAllText(_fullPath);
        
        var handler = JsonSerializer.Deserialize<RemindersHandler>(file);
        
        if (handler == null) return;
        
        Reminders = handler.Reminders;
    }
}