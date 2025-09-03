using Microsoft.AspNetCore.Mvc;
using Timpra.BusinessLogic.DTOs;
using Timpra.BusinessLogic.DTOs.Orders;
using Timpra.BusinessLogic.Helpers.TokenAuthentication;
using Timpra.BusinessLogic.Services.Abstractions;
using System.Net;
using System.Threading.Tasks;

namespace Timpra.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthenticateService _authenticateService;
        private readonly ITokenManager _tokenManager;

        public AuthenticateController(ITokenManager tokenManager, IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
            _tokenManager = tokenManager;
        }

        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(LoginResponseDTO), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login(LoginDTO loginModel)
        {
            if (loginModel == null || string.IsNullOrEmpty(loginModel.Username) || string.IsNullOrEmpty(loginModel.Password))
            {
                return BadRequest("Invalid request.");
            }

            var user = await _authenticateService.Login(loginModel);
            if (user != null)
            {
                return Ok(new LoginResponseDTO { Token = _tokenManager.NewToken(user), FullName = user.FullName, Id = user.Id });
            }
            else
            {
                return Unauthorized("You are not authorized");
            }
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.Created)]
        public async Task<ActionResult> Post([FromBody] UserDTO newUser)
        {
            var result = await _authenticateService.Register(newUser);
            return Ok(result);
        }
    }
}
