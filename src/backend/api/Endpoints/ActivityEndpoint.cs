// TODO: Add authentication + authorization
using api.Entities;
using api.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api.Endpoints;

public static class OrganizationActivityEndpoint
{
    public static RouteGroupBuilder MapOrganizationActivityEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/orgs/{organizationSlug}/activities")
            .WithOpenApi()
            .WithTags("Organization");


        group.MapGet("/by_user/{userId}",OrganizationActivityEndpoint.GetUserActivity)
            .WithOpenApi()
            .WithSummary("List all activities registered to current association for a specific user.");
        group.MapPost("/by_user/{userId}/validate-all", OrganizationActivityEndpoint.ValidateAllUserActivity)
            .WithOpenApi()
            .WithSummary("List all activities registered to current association for a specific user.");

        return group;
    }

    public static async Task<Results<Ok<IEnumerable<Activity>>,BadRequest>> GetUserActivity(string? organizationSlug, int? userId, OrganizationService organizationService)
    {
        if(organizationSlug == "demo")
        {
            return TypedResults.BadRequest();
        }

        var activities = await organizationService.GetUserAllActivitiesAsync(organizationSlug, userId.Value);

        return TypedResults.Ok<IEnumerable<Activity>>(activities);
    }

    public static async Task<Results<Ok, BadRequest>> ValidateAllUserActivity(string organizationSlug, int userId, OrganizationService organizationService)
    {
        if (organizationSlug == "demo")
        {
            return TypedResults.BadRequest();
        }

        await organizationService.ValidateAllUserActivitiesAsync(organizationSlug, userId);

        return TypedResults.Ok();
    }
}
