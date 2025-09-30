using Timpra.BusinessLogic.DTOs.Orders;
using Timpra.DataAccess.Entities;
using System.Threading.Tasks;
using Timpra.BusinessLogic.Helpers.TokenAuthentication;

namespace Timpra.BusinessLogic.Services.Abstractions
{
    public interface IAuthenticateService
    {
        public Task<TokenDTO> Login(LoginDTO loginModel);
        public Task<UserDTO> Register(UserDTO item, bool applyChanges = true);
        public Task<TokenDTO> Refresh(TokenDTO tokenApiDto);
    }
}
