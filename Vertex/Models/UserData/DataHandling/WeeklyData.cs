using Vertex.Models.Interfaces;
using Vertex.Models.UserData.Entry;

namespace Vertex.Models.UserData.DataHandling;

public class WeeklyData : IFileHandler
{
    public List<WeeklyEntry> Snapshot { get; set; }
    
    public void Load()
    {
        
    }

    public void Unload()
    {
        throw new NotImplementedException();
    }
}