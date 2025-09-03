using Timpra.BusinessLogic.DTOs.Orders;
using Timpra.DataAccess.Entities;
using System.Threading.Tasks;

namespace Timpra.BusinessLogic.Services.Abstractions
{
    public interface IAuthenticateService
    {
        public Task<User> Login(LoginDTO loginModel);
        public Task<UserDTO> Register(UserDTO item, bool applyChanges = true);
    }
}
