namespace HelpDesk.Services;

public class EstimationService : IEstimationService
{
    public int EstimateHours(string description, int complexity1to5, int severity0to3)
    {
        var baseHours = Math.Clamp(complexity1to5, 1, 5) * 2; // 2h por ponto de complexidade
        var sevMultiplier = 1.0 + (0.25 * Math.Clamp(severity0to3, 0, 3)); // até 1.75x
        var descBonus = description.Contains("integration", StringComparison.OrdinalIgnoreCase) ? 2 : 0;
        return (int)Math.Ceiling((baseHours + descBonus) * sevMultiplier);
    }
}