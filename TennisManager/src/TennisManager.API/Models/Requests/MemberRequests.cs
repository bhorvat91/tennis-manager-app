using System.ComponentModel.DataAnnotations;
using TennisManager.Domain.Enums;

namespace TennisManager.API.Models.Requests;

public class UpdateMemberRoleRequest
{
    [Required]
    public ClubRole Role { get; set; }
}
