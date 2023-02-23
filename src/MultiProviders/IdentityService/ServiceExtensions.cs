namespace IdentityService;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;

public static class ServiceExtensions
{
    public static void AddIdentityService(this IServiceCollection services)
    {
        var defaultScheme = "Unknown";

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = defaultScheme;
                options.DefaultChallengeScheme = defaultScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic1", config => { })
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler2>("Basic2", config => { })
            .AddPolicyScheme(defaultScheme, defaultScheme, options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    string authorization = context.Request.Headers[HeaderNames.Authorization];
                    if (authorization?.StartsWith("Basic1") == true)
                    {
                        return "Basic1";
                    }
                    return "Basic2";
                };
            });
    }
}
