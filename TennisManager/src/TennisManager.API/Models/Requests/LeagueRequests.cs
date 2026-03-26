using TennisManager.Domain.Enums;
using MatchType = TennisManager.Domain.Enums.MatchType;

namespace TennisManager.API.Models.Requests;

public class CreateLeagueRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public LeagueFormat Format { get; set; } = LeagueFormat.RoundRobin;
    public MatchType MatchType { get; set; } = MatchType.Singles;
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
}

public class UpdateLeagueRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public LeagueFormat? Format { get; set; }
    public MatchType? MatchType { get; set; }
    public LeagueStatus? Status { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
}
