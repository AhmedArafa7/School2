// Infrastructure/TokenService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using School_VerticalSlice.Entities;
using Microsoft.IdentityModel.Tokens;
using School_VerticalSlice.Entities.Enum;

namespace School_VerticalSlice.Infrastructure;

public interface ITokenService
{
    string GenerateAccessToken(User user, int? teacherId, int? studentId);
    string GenerateRefreshToken();
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user, int? teacherId, int? studentId)
    {
        int nameIdentifier = user.Id;
        if (user.Role == Role.Teacher && teacherId != null)
        {
            nameIdentifier = teacherId ?? 0;
        }
        else if (user.Role == Role.Student && studentId != null)
        {
            nameIdentifier = studentId ?? 0;
        }
        var claims = new HashSet<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, nameIdentifier.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new(ClaimTypes.Name, user.Name)
        };


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtSettings:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["jwtSettings:Issuer"],
            audience: _configuration["jwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}