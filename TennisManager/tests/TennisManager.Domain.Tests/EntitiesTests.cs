namespace TennisManager.Domain.Tests;

public class EntitiesTests
{
    [Fact]
    public void ClubMember_DefaultRole_ShouldBePlayer()
    {
        var member = new TennisManager.Domain.Entities.ClubMember();
        Assert.Equal(TennisManager.Domain.Enums.ClubRole.Player, member.Role);
    }
}