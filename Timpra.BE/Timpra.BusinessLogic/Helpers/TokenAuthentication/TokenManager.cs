
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Timpra.DataAccess.Context;
using Timpra.DataAccess.Entities;

namespace Timpra.BusinessLogic.Helpers.TokenAuthentication;
public class TokenManager : ITokenManager
{
    private JwtSecurityTokenHandler tokenHandler;
    private byte[] secretKey = Encoding.ASCII.GetBytes("Timpra-Project-API777777777777777777777777777777777777777777777777777777");
    protected readonly AppDbContext _context;
    private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
    private static readonly int SaltSize = 16;
    private static readonly int HashSize = 20;
    private static readonly int Iterations = 10000;

    public TokenManager(AppDbContext context)
    {
        tokenHandler = new JwtSecurityTokenHandler();
        _context = context;
    }
    public User? Authenticate(string username, string password)
    {
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            var user = _context.Users.Where(b => b.UserName == username && b.Password == password).FirstOrDefault();

            return user;

        }
        return null;
    }

    public string NewToken(User user)
    {
        // TODO: add claims here (full user name & others if needed)
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}") }),
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

    public static string HashPassword(string password)
    {
        byte[] salt;
        rng.GetBytes(salt = new byte[SaltSize]);
        var key = new Rfc2898DeriveBytes(password, salt, Iterations);
        var hash = key.GetBytes(HashSize);

        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        var base64Hash = Convert.ToBase64String(hashBytes);
        return base64Hash;
    }

    public static bool VerifyPassword(string password, string base64Hash)
    {
        var hashBytes = Convert.FromBase64String(base64Hash);

        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        var key = new Rfc2898DeriveBytes(password, salt, Iterations);
        byte[] hash = key.GetBytes(HashSize);

        for (var i = 0; i < HashSize; i++)
        {
            if (hashBytes[i + SaltSize] != hash[i])
            {
                return false;
            }
        }

        return true;

    }

}
