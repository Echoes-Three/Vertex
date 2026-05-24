using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Vertex.Models.Entities;

public static class ActivityColors
{
    public static readonly List<Brush> Palette =
    [
        GetBrush("#C3FE0C"),  // Chartreuse
        GetBrush("#E5F71F"),  // Neon Chartreuse
        GetBrush("#FDF720"),  // Yellow
        GetBrush("#FBD91D"),  // Bright Gold
        GetBrush("#F9BB1A"),  // Amber Gold
        GetBrush("#F79D17"),  // Amber Glow
        GetBrush("#F57F14"),  // Vivid Tangerine
        GetBrush("#F4542B"),  // Blazing Flame
        GetBrush("#EF4012"),  // Blazing Flame
        GetBrush("#D32A26"),  // Flag Red
        GetBrush("#F41C19"),  // Pure Red
        GetBrush("#EA163B"),  // Lipstick Red
        GetBrush("#D9132E"),  // Flag Red
        GetBrush("#B71439"),  // Carmine
        GetBrush("#F94480"),  // Wild Strawberry
        GetBrush("#F95CA2"),  // Deep Pink
        GetBrush("#F82096"),  // Deep Pink
        GetBrush("#C41BB1"),  // Vivid Orchid
        GetBrush("#B015C3"),  // Dark Violet
        GetBrush("#A809DD"),  // Purple (X11)
        GetBrush("#9015CC"),  // Dark Violet
        GetBrush("#8B2AE1"),  // Blue Violet
        GetBrush("#853EF6"),  // Blue Violet
        GetBrush("#5257EA"),  // Majorelle Blue
        GetBrush("#3248D9"),  // Bright Indigo
        GetBrush("#1138C8"),  // Persian Blue
        GetBrush("#1011EB"),  // Blue
        GetBrush("#0C4AF7"),  // Full Spectrum Blue
        GetBrush("#2777E6"),  // Crayola Blue
        GetBrush("#42A3D5"),  // Fresh Sky
        GetBrush("#4CF3FC"),  // Electric Aqua
        GetBrush("#62F8D8"),  // Aquamarine
        GetBrush("#78FCB3"),  // Tropical Mint
        GetBrush("#3CF460"),  // Electric Green
        GetBrush("#00EB0C"),  // Radioactive Grass
        GetBrush("#13FE1D"),  // Lime
        GetBrush("#74FB40"),  // Neon Grass
        GetBrush("#A3FA16"),  // Slime Lime → loops to C3FE0C
    ];
    public static SolidColorBrush GetBrush(string hex) => (SolidColorBrush)new BrushConverter().ConvertFromString(hex)!;
}