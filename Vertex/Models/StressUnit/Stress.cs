using Vertex.Models.EnumDefinitions;
using static System.Math;

namespace Vertex.Models.StressRiskUnit;

public static class Stress
{
    // Converts user entry daya into Stress unit
    public static (double Score, string Id) ConvertToScore(
        RequiredEnergy energy,
        ExpectedEnjoyment enjoyment,
        int duration,
        string id)
    {
        var delta = (int)energy - (int)enjoyment;
        var stress = delta * (1 - Exp(-0.7 * duration)) + 0.4 * delta * duration;
        var score = 5 + 5 * Tanh(stress);
        return ( score, id);
    }
}