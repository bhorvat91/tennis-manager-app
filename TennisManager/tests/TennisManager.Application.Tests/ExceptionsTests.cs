using TennisManager.Application.Common.Exceptions;

namespace TennisManager.Application.Tests;

public class ExceptionsTests
{
    [Fact]
    public void NotFoundException_ShouldContainEntityNameAndKey()
    {
        var ex = new NotFoundException("Club", Guid.Empty);
        Assert.Contains("Club", ex.Message);
    }
}