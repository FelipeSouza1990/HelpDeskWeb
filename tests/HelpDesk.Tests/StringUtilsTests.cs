using HelpDesk.Legacy;
using Xunit;

namespace HelpDesk.Tests;

public class StringUtilsTests
{
    [Fact]
    public void Truncate_ReturnsEmpty_OnNull()
    {
        var s = StringUtils.Truncate(null, 5);
        Assert.Equal(string.Empty, s);
    }

    [Fact]
    public void Truncate_Cuts_WhenLonger()
    {
        var s = StringUtils.Truncate("abcdef", 3);
        Assert.Equal("abc", s);
    }
}