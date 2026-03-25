using TennisManager.API.Models;

namespace TennisManager.API.Tests;

public class MiddlewareTests
{
    [Fact]
    public void ErrorResponse_ShouldHaveCorrectDefaultValues()
    {
        var response = new ErrorResponse { StatusCode = 404, Message = "Not found" };
        Assert.Equal(404, response.StatusCode);
        Assert.Equal("Not found", response.Message);
    }
}