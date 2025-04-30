using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using AuthenticationAPI.Model;
using AuthenticationAPI.Interfaces;

namespace AuthenticationAPI.Authentication;

public class JwtTokenProvider(IConfiguration configuration) : IJwtTokenProvider
{
    public async Task<string> GenerateJwtTokenAsync(LoginDetail user)
    {
        ArgumentNullException.ThrowIfNull(user);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.UserName)
        ];

        // Add roles and permissions claims
        claims.Add(new Claim("permissions", string.Join(",", ["Read", "Write"])));

        SigningCredentials signingCredentials = new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])), SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(configuration["Jwt:Issuer"], configuration["Jwt:Audience"], claims, signingCredentials: signingCredentials, expires: DateTime.UtcNow.AddMinutes(5));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
