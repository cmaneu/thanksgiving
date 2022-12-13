using api.Services;

namespace api.Endpoints;

public record LoginResponse
{
    public LoginResponse()
    { }
    
    public LoginResponse(AuthToken authorizationToken, string jwtToken)
    {
        AuthorizationToken = authorizationToken;
        JwtToken = jwtToken;
    }

    public AuthToken? AuthorizationToken { get; init; }
    public string? JwtToken { get; init; }
}

public record LoginRequest
{
    public required string UserEmail { get; init; }
}


public record RefreshTokenResponse
{
    public string RefreshToken { get; set; }
}