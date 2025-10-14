
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Timpra.DataAccess.Context;
using Timpra.DataAccess.Entities;

namespace Timpra.BusinessLogic.Helpers.TokenAuthentication;
public class TokenManager : ITokenManager
{
    
    protected readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public TokenManager(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public byte[] GetSecretKey()
    {
        return Encoding.ASCII.GetBytes(_configuration.GetConnectionString("SigningSecretKey"));
    }

    public string NewToken(User user)
    {
        // TODO: add claims here (full user name & others if needed)
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var identity = new ClaimsIdentity(new Claim[]
        {
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, $"{user.Id}")
        });

        var credentials = new SigningCredentials(new SymmetricSecurityKey(GetSecretKey()), SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = identity,
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = credentials
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        return jwtTokenHandler.WriteToken(token);
    }

    public string NewToken()
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(64);
        var refreshToken = Convert.ToBase64String(tokenBytes);
        return refreshToken;
    }

    public ClaimsPrincipal VerifyToken(string tokenValue)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(GetSecretKey()),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(tokenValue, tokenValidationParameters, out securityToken); //out -> passed by reference, it will be modified by the method
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("This is invalid token");
        return principal;
    }
}
