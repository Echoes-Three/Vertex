using System.Windows.Media;

namespace Vertex.Data.Services;

public static class Colors
{
    public static readonly List<Brush> Palette =
    [
        GetBrush("#C3FE0C"),  // Chartreuse
        GetBrush("#FDF720"),  // Yellow
        GetBrush("#F4542B"),  // Blazing Flame
        GetBrush("#EA163B"),  // Lipstick Red
        GetBrush("#F82096"),  // Deep Pink
        GetBrush("#A809DD"),  // Purple (X11)
        GetBrush("#853EF6"),  // Blue Violet
        GetBrush("#1011EB"),  // Blue
        GetBrush("#0C4AF7"),  // Full Spectrum Blue
        GetBrush("#4CF3FC"),  // Electric Aqua
        GetBrush("#78FCB3"),  // Tropical Mint
        GetBrush("#00EB0C"),  // Radioactive Grass
    ];
    public static SolidColorBrush GetBrush(string hex) => (SolidColorBrush)new BrushConverter().ConvertFromString(hex)!;
}