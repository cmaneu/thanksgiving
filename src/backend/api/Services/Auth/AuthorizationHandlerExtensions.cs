using Microsoft.AspNetCore.Authorization;

namespace api.Services.Auth;

public static class AuthorizationHandlerExtensions
{
    public static AuthorizationBuilder AddCurrentUserHandler(this AuthorizationBuilder builder)
    {
        builder.Services.AddScoped<IAuthorizationHandler, CheckCurrentUserAuthHandler>();
        return builder;
    }

    public static AuthorizationPolicyBuilder RequireCurrentUser(this AuthorizationPolicyBuilder builder)
    {
        return builder.RequireAuthenticatedUser()
                      .AddRequirements(new CheckCurrentUserRequirement());
    }

    private class CheckCurrentUserRequirement : IAuthorizationRequirement { }

    private class CheckCurrentUserAuthHandler : AuthorizationHandler<CheckCurrentUserRequirement>
    {
        private readonly CurrentIdentity _currentUser;
        public CheckCurrentUserAuthHandler(CurrentIdentity currentUser) => _currentUser = currentUser;

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckCurrentUserRequirement requirement)
        {
            // TODO: Check user if the user is locked out as well
            if (_currentUser.User is not null)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}