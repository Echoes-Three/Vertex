using System.Windows.Media;
using Vertex.Models.EnumDefinitions;

namespace Vertex.Models;

public class ActivityEntry
{
    public string Id { get; set; }
    public Color Color {get; set;}
    public string Name { get; set; }
    public bool Completed { get; set; }
    public TimeSpan DurationHours { get; set; }
    public RequiredEnergy Energy  { get; set; }
    public ExpectedEnjoyment Enjoyment { get; set; }
    public int PlacementOrder { get; set; }
}