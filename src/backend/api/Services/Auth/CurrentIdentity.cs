using api.ApplicationCore.Entities;
using System.Security.Claims;

namespace api.Services.Auth;

public record CurrentIdentity
{
    public string OriginalTokenId { get; set; }
    public User User { get; set; }
    public ClaimsPrincipal Principal { get; set; }
    public bool HasAdminAccess(string organizationName) => Principal.HasClaim("org:admin", organizationName);
    public bool HasMemberAccess(string organizationName) => Principal.HasClaim("org:volunteer", organizationName) || HasAdminAccess(organizationName);
}

