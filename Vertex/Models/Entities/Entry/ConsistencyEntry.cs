using Vertex.Models.Interfaces;

namespace Vertex.Models.Entities.Entry;

public class ConsistencyEntry(
    List<int> currentWeek = null,
    List<int> lastWeek = null)
{
    public List<int>? CurrentWeek { get; set; } = currentWeek;
    public List<int>? LastWeek { get; set; } = lastWeek;
}