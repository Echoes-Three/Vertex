using System.IO;
using System.Text.Json;
using Vertex.Models.Interfaces;

namespace Vertex.Models.DataServices.DataHandling;

public class ConsistencyHandler(
    List<int> currentWeek,
    List<int> priorWeekOne,
    List<int> priorWeekTwo)
    : IFileHandler
{
    public ConsistencyHandler() : this([], [], []) 
    {
    }
    
    public List<int> CurrentWeek { get; set; } = currentWeek;
    public List<int> PriorWeekOne { get; set; } = priorWeekOne;
    public List<int> PriorWeekTwo { get; set; } = priorWeekTwo;
    
    public void Save(int percentage)
    {
        
        if (CurrentWeek.Count == 7)
        {
            PriorWeekTwo = PriorWeekOne;
            PriorWeekOne = CurrentWeek;
            CurrentWeek.Clear();
        }
        
        CurrentWeek.Add(percentage);
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
        
        CurrentWeek = weeks.CurrentWeek;
        PriorWeekOne = weeks.PriorWeekOne;
        PriorWeekTwo = weeks.PriorWeekTwo;
        
    }
}