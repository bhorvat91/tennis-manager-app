namespace TennisManager.Domain.Tests;

public class EntitiesTests
{
    [Fact]
    public void ClubMember_DefaultRole_ShouldBePlayer()
    {
        var member = new TennisManager.Domain.Entities.ClubMember();
        Assert.Equal(TennisManager.Domain.Enums.ClubRole.Player, member.Role);
    }

    [Fact]
    public void Reservation_DefaultStatus_ShouldBeConfirmed()
    {
        var reservation = new TennisManager.Domain.Entities.Reservation();
        Assert.Equal(TennisManager.Domain.Enums.ReservationStatus.Confirmed, reservation.Status);
    }

    [Fact]
    public void Reservation_DefaultType_ShouldBeOther()
    {
        var reservation = new TennisManager.Domain.Entities.Reservation();
        Assert.Equal(TennisManager.Domain.Enums.ReservationType.Other, reservation.ReservationType);
    }

    [Fact]
    public void Reservation_DefaultParticipants_ShouldBeEmptyList()
    {
        var reservation = new TennisManager.Domain.Entities.Reservation();
        Assert.NotNull(reservation.Participants);
        Assert.Empty(reservation.Participants);
    }

    [Fact]
    public void ReservationParticipant_UserAndReservationNavigation_ShouldBeNullByDefault()
    {
        var participant = new TennisManager.Domain.Entities.ReservationParticipant();
        Assert.Equal(Guid.Empty, participant.ReservationId);
        Assert.Equal(Guid.Empty, participant.UserId);
    }

    [Fact]
    public void League_DefaultStatus_ShouldBeDraft()
    {
        var league = new TennisManager.Domain.Entities.League();
        Assert.Equal(TennisManager.Domain.Enums.LeagueStatus.Draft, league.Status);
    }

    [Fact]
    public void League_DefaultFormat_ShouldBeRoundRobin()
    {
        var league = new TennisManager.Domain.Entities.League();
        Assert.Equal(TennisManager.Domain.Enums.LeagueFormat.RoundRobin, league.Format);
    }

    [Fact]
    public void League_DefaultParticipants_ShouldBeEmptyList()
    {
        var league = new TennisManager.Domain.Entities.League();
        Assert.NotNull(league.Participants);
        Assert.Empty(league.Participants);
    }

    [Fact]
    public void League_DefaultLeagueMatches_ShouldBeEmptyList()
    {
        var league = new TennisManager.Domain.Entities.League();
        Assert.NotNull(league.LeagueMatches);
        Assert.Empty(league.LeagueMatches);
    }

    [Fact]
    public void LeagueMatch_DefaultIsRequired_ShouldBeTrue()
    {
        var leagueMatch = new TennisManager.Domain.Entities.LeagueMatch();
        Assert.True(leagueMatch.IsRequired);
    }
}