using HelpDesk.Models;
using Xunit;

namespace HelpDesk.Tests;

public class StatusTransitionTests
{
    [Theory]
    [InlineData(TicketStatus.Open, TicketStatus.InProgress, true)]
    [InlineData(TicketStatus.Open, TicketStatus.Resolved, false)]
    [InlineData(TicketStatus.InProgress, TicketStatus.Resolved, true)]
    [InlineData(TicketStatus.Resolved, TicketStatus.Closed, true)]
    [InlineData(TicketStatus.Closed, TicketStatus.Open, false)]
    public void CanTransitionTo_Works(TicketStatus from, TicketStatus to, bool expected)
    {
        Assert.Equal(expected, from.CanTransitionTo(to));
    }
}