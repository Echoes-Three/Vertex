using System.IO;
using System.Text.Json;
using Vertex.Models.Interfaces;
using Vertex.Models.UserData.Entry;

namespace Vertex.Models.UserData.DataHandling;

public class ConsistencyHandler(
    List<int> currentWeek,
    List<int> priorWeekOne,
    List<int> priorWeekTwo)
    : IFileHandler
{
    public List<int> currentWeek { get; set; } = currentWeek;
    public List<int> priorWeekOne { get; set; } = priorWeekOne;
    public List<int> priorWeekTwo { get; set; } = priorWeekTwo;

    public void Save(int percentage)
    {
        
        if (currentWeek.Count == 7)
        {
            priorWeekTwo = priorWeekOne;
            priorWeekOne = currentWeek;
            currentWeek.Clear();
        }
        
        currentWeek.Add(percentage);
    }

    public void Load()
    {
        var fullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex", "Data", "Weeks.json"
        );
        
        var file = File.ReadAllText(fullPath);
        
        var weeks = JsonSerializer.Deserialize<ConsistencyHandler>(file);
        
        if (weeks == null) return;
        
        currentWeek = weeks.currentWeek;
        priorWeekOne = weeks.priorWeekOne;
        priorWeekTwo = weeks.priorWeekTwo;
        
    }
}