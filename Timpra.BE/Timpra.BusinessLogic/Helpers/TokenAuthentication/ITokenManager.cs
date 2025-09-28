using System.Security.Claims;
using System.Threading.Tasks;
using Timpra.DataAccess.Entities;

namespace Timpra.BusinessLogic.Helpers.TokenAuthentication;

public interface ITokenManager
{
    string NewToken(User user);
    ClaimsPrincipal VerifyToken(string tokenValue);
    string NewToken();
}