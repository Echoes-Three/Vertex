using Vertex.Models.Interfaces;

namespace Vertex.Models.Entities.Entry;

public class ConsistencyEntry
{
    public List<int> CurrentWeek { get; set; } = null ?? Enumerable.Repeat(0, 7).ToList();
    public List<int> LastWeek { get; set; } = null ?? Enumerable.Repeat(0, 7).ToList();
}