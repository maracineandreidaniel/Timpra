using Timpra.BusinessLogic.DTOs.Orders;
using Timpra.BusinessLogic.Helpers.TokenAuthentication;
using Timpra.BusinessLogic.Services.Abstractions;
using Timpra.DataAccess.Entities;
using Timpra.DataAccess.Repository.Abstraction;
using System.Threading.Tasks;
using Timpra.BusinessLogic.Mappers;

namespace Timpra.BusinessLogic.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IRepository<User> _userRepository;
        private readonly ITokenManager _tokenManager;

        public AuthenticateService(IRepository<User> userRepository, ITokenManager tokenManager)
        {
            _userRepository = userRepository;
            _tokenManager = tokenManager;
        }

        public async Task<User> Login(LoginDTO loginModel)
        {
            var user = _tokenManager.Authenticate(loginModel.Username, loginModel.Password);
            return await Task.FromResult(user);
        }

        public async Task<UserDTO> Register(UserDTO item, bool applyChanges = true)
        {
            await _userRepository.AddAsync(item.MapFromDto());

            return item;
        }
    }
}