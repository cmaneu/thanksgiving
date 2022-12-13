using api.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.FeatureManagement;
using Microsoft.AspNetCore.RateLimiting;
using api.Services;
using api.Services.Auth;

namespace api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/.auth")
            .WithOpenApi()
            .WithTags("Authentication");

        // TODO: POST /login (userEmail)
        // - Get User from DB
        // - Create verify token in DB
        // - Send email
        group.MapPost("/login", async Task<Results<Ok<LoginResponse>,BadRequest>> (LoginRequest request, ILogger<Program> logger, IFeatureManager featureManager, AuthorizationService authService) =>
        {
            logger.LogInformation($"Login request for {request.UserEmail}");
            var authToken = await authService.StartUserLoginWithValidationToken(request.UserEmail);
            
            if (authToken != null && await featureManager.IsEnabledAsync(FeatureFlags.AuthBypassEmailDirectResponse))
            {
                return TypedResults.Ok(new LoginResponse(authToken?.token,authToken?.jwt));
            }

            // We always return an OK result, even if the email does not exist. This prevents
            // an attacker from determining which emails are registered in the system.
            return TypedResults.Ok(new LoginResponse());
        })
        .WithOpenApi()
        .WithDisplayName("Start signin process")
        .WithSummary("Generate an OTP for login an user")
        ;

        // TODO: All the auth handler extensions / current user extensions.

        group.MapGet("/verify-token", async (string? t, bool? consume, AuthorizationService authService) =>
        {
            if(consume == true)
            {
                // Auth Exchange token
                var newTokens = await authService.ContinueUserLoginWithValidationToken(t);
                if (!newTokens.HasValue || string.IsNullOrEmpty(newTokens.Value.refreshToken))
                    return Results.BadRequest();

                return new CookieJSONResponse(new RefreshTokenResponse { RefreshToken = newTokens.Value.refreshToken }, "device_token", newTokens.Value.deviceToken);
                
            }
            else
            {
                // TODO: Check token
            }
            return Results.BadRequest();
        }).WithOpenApi()
        .WithDisplayName("Continue signin process")
        .WithSummary("Generate device and refresh token");

        // TODO: GET /verify-token?t={validation-token}&consume=false
        // - Validate JWT Token
        // - Delete token from DB
        // - Generate refresh token and session token
        // - Return both
        // - Save refresh token in DB

        group.MapGet("/refresh-token", async (HttpRequest request,CurrentIdentity currentId, AuthorizationService authService) =>
        {
            var deviceToken = request.Cookies["device_token"];
            
            // Refresh token has been validated by Authorization middleware, so we just have to check device token.
            if (await authService.ValidateDeviceToken(deviceToken))
            {
                // Auth Exchange token
                var newTokens = await authService.RefreshTokens(deviceToken, currentId.OriginalTokenId);
                return new CookieJSONResponse(new RefreshTokenResponse { RefreshToken = newTokens.Value.refreshToken }, "device_token", newTokens.Value.deviceToken);
            }
            
            return Results.BadRequest();
        }).WithOpenApi()
        .WithDisplayName("Continue signin process")
        .WithSummary("Generate device and refresh token")
        .RequireAuthorization();

        // TODO: POST /verify-token/{tokenId} (with OTP)
        // TODO: POST /register (userEmail)
        return group;
    }
}


