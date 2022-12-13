using api.ApplicationCore.Entities;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace api.Services.Auth;

public static class CurrentIdentityExtensions
{
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddScoped<CurrentIdentity>();
        services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
        return services;
    }

    private sealed class ClaimsTransformation : IClaimsTransformation
    {
        private readonly CurrentIdentity _currentIdentity;
        //private readonly UserManager<AppUser> _userManager;

        public ClaimsTransformation(CurrentIdentity currentIdentity) //UserManager<AppUser> userManager
        {
            _currentIdentity = currentIdentity;
            //_userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // We're not going to transform anything. We're using this as a hook into authorization
            // to set the current user without adding custom middleware.
            _currentIdentity.Principal = principal;

            _currentIdentity.OriginalTokenId = principal.FindFirstValue("tkid");
            _currentIdentity.User = new User()
            {
                Id = int.Parse(principal.FindFirstValue("uid")),
                FirstName = principal.FindFirstValue(ClaimTypes.Name)
            };

            //if (principal.FindFirstValue(ClaimTypes.NameIdentifier) is { Length: > 0 } name)
            //{
            //    // Resolve the user manager and see if the current user is a valid user in the database
            //    // we do this once and store it on the current user.
            //    //_currentUser.User = await _userManager.FindByNameAsync(name);
         
            //}

            return principal;
        }
    }
}
