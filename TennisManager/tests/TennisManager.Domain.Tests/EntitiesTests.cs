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
}