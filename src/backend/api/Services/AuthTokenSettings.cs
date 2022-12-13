using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace api.Services;

public record AuthTokenSettings
{
    public string? JwtSigningKey { get; init; }

    public byte[] JwtSigningKeyAsBytes => Encoding.UTF8.GetBytes(JwtSigningKey);

    public string DefaultIssuer { get; set; }
    public string APIAudience { get; set; }
}