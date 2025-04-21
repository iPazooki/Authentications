using AuthenticationAPI.Model;

namespace AuthenticationAPI.Interfaces;

/// <summary>
/// Interface for generating JWT tokens.
/// </summary>
public interface IJwtTokenProvider
{
    Task<string> GenerateJwtTokenAsync(LoginDetail user);
}
