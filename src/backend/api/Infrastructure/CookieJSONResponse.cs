using Microsoft.AspNetCore.Http.Metadata;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace api.Infrastructure
{
    public class CookieJSONResponse : IResult, IEndpointMetadataProvider
    {
        public object? Value { get; }
        public string CookieName { get; }
        public string CookieValue { get; }

        public CookieJSONResponse(object? value, string? cookieName, string? cookieValue)
        {
            Value = value;
            CookieName = cookieName;
            CookieValue = cookieValue;
        }
        
        public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
        {
            //builder.Metadata.Add(new ProducesHtmlMetadata());
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = MediaTypeNames.Application.Json;
            if (CookieName != null && CookieValue != null)
            {
                httpContext.Response.Cookies.Append(CookieName, CookieValue, new CookieOptions() { Secure = true, SameSite = SameSiteMode.Lax, IsEssential = true }) ;
            }
            return httpContext.Response.WriteAsync(JsonSerializer.Serialize(Value));
        }
    }

}
