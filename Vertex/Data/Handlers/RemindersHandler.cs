using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Vertex.Models.Contracts;
using Vertex.Models.Entities;

namespace Vertex.Data.Handlers;

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
        if (!File.Exists(_fullPath))
        {
            Reminders = new ObservableCollection<ReminderEntry>();
            return;
        }

        try
        {
            var file = File.ReadAllText(_fullPath);

            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
            };

            var handler = JsonSerializer.Deserialize<RemindersHandler>(file, options);
            Reminders = handler?.Reminders ?? new ObservableCollection<ReminderEntry>();
        }
        catch (JsonException)
        {
            Reminders = new ObservableCollection<ReminderEntry>();
        }
    }
}