using api.ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.FeatureManagement.FeatureFilters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace api.Services;

public interface ITokenService
{
    public Task<AuthToken> GenerateToken(TokenType type, User user);
    public string AsJwtToken(AuthToken token);
    public Task<AuthToken> GenerateAuthorizationToken(User user);
    Task<string?> ValidateToken(string validationToken, TokenType type);
    Task ConsumeToken(string validationToken);
    Task ConsumeTokenByIdAsync(string tokenId);
    Task <AuthToken> GetById(string tokenId);
}

public class TokenService : ITokenService
{
    IAuthenticationTokenStore _tokenStore;
    private AuthTokenSettings _authTokenSettings;
    private JwtSecurityTokenHandler _tokenHandler;
    private SymmetricSecurityKey _signingKey;
    private SigningCredentials _signingCredentials;

    public TokenService(IAuthenticationTokenStore tokenStore, IConfiguration config)
    {
        this._tokenStore = tokenStore;
        this._authTokenSettings = config.GetSection(nameof(AuthTokenSettings)).Get<AuthTokenSettings>();

        _tokenHandler = new JwtSecurityTokenHandler();
        _signingKey = new SymmetricSecurityKey(_authTokenSettings.JwtSigningKeyAsBytes);
        _signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha512Signature);
    }

    public string AsJwtToken(AuthToken token)
    {
        ClaimsIdentity claims = new ClaimsIdentity(new Claim[] {
            new Claim(ClaimTypes.Name,token.UserName),
            new Claim("uid",token.UserId),
            new Claim("tkid",token.Id),
        });

        // TODO: Implement custom claims for 
        string audience = token.Type switch
        {
            TokenType.AuthorizationToken => "auth:authorize",
            TokenType.DeviceToken => "auth:authorize",
            TokenType.RefreshToken => "api",
            _ => throw new ApplicationException("Token type not handled by Token service")
        };
        
        var jwtToken = _tokenHandler.CreateJwtSecurityToken(_authTokenSettings.DefaultIssuer, audience, claims, null, token.ValidUntil, DateTime.Now, _signingCredentials);
        return jwtToken.RawData;
    }

    public async Task ConsumeToken(string validationToken)
    {
        var jwtToken = _tokenHandler.ReadJwtToken(validationToken);
        var tokenIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "tkid");
        await _tokenStore.DeleteToken(tokenIdClaim.Value);
    }

    public async Task ConsumeTokenByIdAsync(string tokenId)
    {
        await _tokenStore.DeleteToken(tokenId);
    }

    public Task<AuthToken> GenerateAuthorizationToken(User user)
    {
        return GenerateToken(TokenType.AuthorizationToken, user);
    }

    public async Task<AuthToken> GenerateToken(TokenType type, User user)
    {
        AuthToken token = new AuthToken()
        {
            Type = type,
            Id = Guid.NewGuid().ToString("N"),
            UserId = user.Id.ToString(),
            UserName = user.FirstName,
            // TODO: Replace DateTime by a time interface
            CreatedAt = DateTime.UtcNow,
            Validity = GetDefaultValidityByTokenType(type)
        };

        await _tokenStore.SetTokenAsync(token.Id, token);
        
        return token;
    }

    public async Task<AuthToken> GetById(string tokenId)
    {
        return await _tokenStore.GetToken(tokenId);
    }

    public async Task<string?> ValidateToken(string token, TokenType type)
    {
        // validate validationtoken signature
        try
        {
            _tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _authTokenSettings.DefaultIssuer,
                ValidAudience = GetAudienceByTokenType(type),
                IssuerSigningKey = _signingKey
            }, out SecurityToken validatedToken);
            
            var readToken = _tokenHandler.ReadJwtToken(token);

            var tokenIdClaim = readToken?.Claims.FirstOrDefault(c => c.Type == "tkid");

            // Check if validation token exists in DB and is still valid
            var storedToken = await _tokenStore.GetToken(tokenIdClaim?.Value);

            if (storedToken != null && storedToken.ValidUntil > DateTime.UtcNow)
                return tokenIdClaim?.Value;
        }
        catch
        {
            return null;
        }
        return null;
    }

    private string GetAudienceByTokenType(TokenType type)
    {
        switch (type)
        {
            case TokenType.DeviceToken:
            case TokenType.AuthorizationToken:
                return "auth:authorize";
            case TokenType.RefreshToken:
                return _authTokenSettings.APIAudience;
            default:
                return "unknown";
        }
    }

    private TimeSpan GetDefaultValidityByTokenType(TokenType type)
    {
        switch (type)
        {
            case TokenType.AuthorizationToken:
                return TimeSpan.FromMinutes(15);
            case TokenType.RefreshToken:
                return TimeSpan.FromMinutes(60);
            case TokenType.DeviceToken:
                return TimeSpan.FromDays(30);
            default:
                return TimeSpan.FromMinutes(15);
        }
    }
}
