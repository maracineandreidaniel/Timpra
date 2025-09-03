
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Timpra.BusinessLogic.Helpers.TokenAuthentication;
using System;
using System.Linq;

namespace Timpra.API.Filters;
public class TokenAuthenticationFilter : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var tokenManager = context.HttpContext.RequestServices.GetService(typeof(ITokenManager)) as ITokenManager;


        var result = true;

        if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            result = false;

        string token = string.Empty;
        if (result == true)
        {
            token = context.HttpContext.Request.Headers.First(x => x.Key == "Authorization").Value;
            token = token.Replace("Bearer ", "");

            try
            {
                var claimPrincipal = tokenManager.VerifyToken(token);

            }
            catch (Exception ex)
            {
                result = false;
                context.ModelState.AddModelError("Unauthorized", ex.ToString());
            }

        }

        if (!result) context.Result = new UnauthorizedObjectResult(context.ModelState);

    }
}
