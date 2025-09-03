using Timpra.DataAccess.Entities;
using System.Security.Claims;

namespace Timpra.BusinessLogic.Helpers.TokenAuthentication;

public interface ITokenManager
{
    User? Authenticate(string username, string password);
    string NewToken(User user);
    ClaimsPrincipal VerifyToken(string tokenValue);
}