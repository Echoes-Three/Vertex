using Vertex.Models.EnumDefinitions;
using static System.Math;

namespace Vertex.Models.StressUnit;

public static class Stress
{
    public static double ToScore(
        RequiredEnergy energy,
        ExpectedEnjoyment enjoyment,
        TimeSpan duration)
    {
        var h = (int)duration.TotalHours;
        var delta = (int)energy - (int)enjoyment;
        var stress = delta * (1 - Exp(-0.7 * h)) + 0.4 * delta * h;
        var score = 5 + 5 * Tanh(stress);
        
        return score;
    }
}