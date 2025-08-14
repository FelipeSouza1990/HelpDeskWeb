using HelpDesk.Services;
using Xunit;

namespace HelpDesk.Tests;

public class EstimationServiceTests
{
    [Theory]
    [InlineData("simple", 1, 0, 2)]
    [InlineData("integration issue", 3, 2, 9)] // 3*2=6 +2 = 8 *1.5 = 12 -> ceil 12 (ajuste abaixo)
    public void EstimateHours_ReturnsExpected(string desc, int complexity, int severity, int expectedMin)
    {
        var svc = new EstimationService();
        var hours = svc.EstimateHours(desc, complexity, severity);
        Assert.True(hours >= expectedMin);
    }
}