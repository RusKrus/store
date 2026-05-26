using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Store.Infrastructure.Auth;

public class JwtProvider(IConfiguration configuration) : IJwtProvider
{
    public string Generate(User user)
    {
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),
            new Claim(
                ClaimTypes.Email,
                user.Email),
            new Claim(
                ClaimTypes.Role,
                user.Role.ToString()),
            new Claim(
                ClaimTypes.Name,
                $"{user.LastName} {user.FirstName}"
                ),
            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration.GetSection("JwtKey").Value)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration.GetSection("JwtIssuer").Value,
            audience: configuration.GetSection("JwtAudience").Value,
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(1));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}