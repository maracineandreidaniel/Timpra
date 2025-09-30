using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Timpra.BusinessLogic.DTOs;
using Timpra.BusinessLogic.DTOs.Orders;
using Timpra.BusinessLogic.Exceptions;
using Timpra.BusinessLogic.Helpers.TokenAuthentication;
using Timpra.BusinessLogic.Services.Abstractions;

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

            var result = await _authenticateService.Login(loginModel);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return Unauthorized("You are not authorized");
            }
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.NotImplemented)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> Post([FromBody] UserDTO newUser)
        {
            var result = await _authenticateService.Register(newUser);
            return Ok(result);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(TokenDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Refresh([FromBody] TokenDTO tokenApiDto)
        {
            try
            {
                var result = await _authenticateService.Refresh(tokenApiDto);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (InvalidRequestException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch
            {
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }
    }
}
