
using Microsoft.IdentityModel.Tokens;
using Timpra.DataAccess.Context;
using Timpra.DataAccess.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Timpra.BusinessLogic.Helpers.TokenAuthentication;
public class TokenManager : ITokenManager
{
    private JwtSecurityTokenHandler tokenHandler;
    private byte[] secretKey = Encoding.ASCII.GetBytes("Timpra-Project-API777777777777777777777777777777777777777777777777777777");
    protected readonly AppDbContext _context;


    public TokenManager(AppDbContext context)
    {
        tokenHandler = new JwtSecurityTokenHandler();
        _context = context;
    }
    public User? Authenticate(string username, string password)
    {
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            var user = _context.Users.Where(b => b.Username == username && b.Password == password).FirstOrDefault();

            return user;

        }
        return null;
    }

    public string NewToken(User user)
    {
        // TODO: add claims here (full user name & others if needed)
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, user.FullName) }),
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature
                )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwtString = tokenHandler.WriteToken(token);

        return jwtString;
    }

    public ClaimsPrincipal VerifyToken(string tokenValue)
    {
        var claims = tokenHandler.ValidateToken(tokenValue,
            new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ClockSkew = TimeSpan.Zero
            },
            out SecurityToken validateToken
            );
        return claims;
    }

}
