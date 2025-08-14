namespace HelpDesk.Services;

public interface IEstimationService
{
    int EstimateHours(string description, int complexity1to5, int severity0to3);
}