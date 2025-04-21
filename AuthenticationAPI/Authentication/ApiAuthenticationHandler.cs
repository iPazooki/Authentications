using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;

namespace AuthenticationAPI.Authentication;

public class ApiAuthenticationHandler : AuthenticationHandler<ApiAuthenticationHandlerOptions>
{
    public ApiAuthenticationHandler(
        IOptionsMonitor<ApiAuthenticationHandlerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
        // Implement your custom authentication logic here
        // For example, validate the token and create a ClaimsPrincipal

        // If authentication is successful, return AuthenticateResult.Success(ticket);
        // If authentication fails, return AuthenticateResult.Fail(exception);
        // If no authentication is performed, return AuthenticateResult.NoResult();

        if (!Request.Headers.TryGetValue("Api-Key", out var customToken))
            return AuthenticateResult.NoResult();

        if (customToken == "Api-Key")
        {
            // Create claims and principal
            var claims = new[] { new Claim("name", "CustomUser") };
            var identity = new ClaimsIdentity(claims, "Custom");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Custom");

            // Return success
            return AuthenticateResult.Success(ticket);
        }
        else
        {
            return AuthenticateResult.Fail("Invalid AccessToken");
        }
    }
}

public class ApiAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
    // Add any custom options you need for your authentication handler
    // For example, you can add properties for token validation parameters
}