using api.ApplicationCore.Entities;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Cryptography.X509Certificates;

namespace api.Services;

public class AuthorizationService
{
    IUserRepository _userRepository;
    ITokenService _tokenService;

    public AuthorizationService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<(AuthToken token, string jwt)?> StartUserLoginWithValidationToken(string userEmail)
    {
        User? user = await _userRepository.GetByEmailAsync(userEmail);
        
        if(user == null)
            return null;

        AuthToken token = await _tokenService.GenerateAuthorizationToken(user);
        string tokenJwt = _tokenService.AsJwtToken(token);

        // TODO: Send an email

        return (token,tokenJwt);
    }

    internal async Task<(string deviceToken, string refreshToken)?> ContinueUserLoginWithValidationToken(string validationToken)
    {
        string? tokenId = await _tokenService.ValidateToken(validationToken, TokenType.AuthorizationToken);
        if (string.IsNullOrEmpty(tokenId))
            return (null,null);

        AuthToken dbToken = await _tokenService.GetById(tokenId);

        // Delete from DB
        await _tokenService.ConsumeToken(validationToken);
        
        User user = await _userRepository.GetByIdAsync(dbToken.UserId);

        // Generate device token
        var deviceToken = await _tokenService.GenerateToken(TokenType.DeviceToken, user);
        var refreshToken = await _tokenService.GenerateToken(TokenType.RefreshToken, user);
        // Generate refresh token
        return ( _tokenService.AsJwtToken(deviceToken), _tokenService.AsJwtToken(refreshToken));
    }

    internal async Task<bool> ValidateDeviceToken(string token)
    {
        string? tokenId = await _tokenService.ValidateToken(token, TokenType.DeviceToken);
        return !string.IsNullOrEmpty(tokenId);
    }
    
    internal async Task<(string deviceToken, string refreshToken)?> RefreshTokens(string deviceToken, string refreshTokenId)
    {
        string? deviceTokenId = await _tokenService.ValidateToken(deviceToken, TokenType.DeviceToken);
        

        if (string.IsNullOrEmpty(deviceTokenId))
            return (null, null);

        AuthToken dbToken = await _tokenService.GetById(deviceTokenId);
        User user = await _userRepository.GetByIdAsync(dbToken.UserId);
        
        await _tokenService.ConsumeTokenByIdAsync(deviceTokenId);
        await _tokenService.ConsumeTokenByIdAsync(refreshTokenId);

        var newDevicetoken = await _tokenService.GenerateToken(TokenType.DeviceToken, user);
        var newRefreshToken = await _tokenService.GenerateToken(TokenType.RefreshToken, user);

        return (_tokenService.AsJwtToken(newDevicetoken), _tokenService.AsJwtToken(newRefreshToken));
    }
}
