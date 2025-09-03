using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Timpra.API.Middleware
{
    /// <summary>
    /// Custom middleware to bypass the CORS validation on the OPTIONS request method (verb)
    /// </summary>
    public class OptionsMiddleware
    {
        #region Private data
        private readonly RequestDelegate _next;

        #endregion

        public OptionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // if method is options, bypass CORS 
            if (context.Request.Method == "OPTIONS")
            {
                // these headers have to be manually set. the ones from the CORS policy are not taken into account, because the response is  manually set (see the next two lines)
                context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)context.Request.Headers["Origin"] });
                context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin, Access-Control-Allow-Headers, X-Requested-With, Authorization, Content-Type, Accept" });
                context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, DELETE, PUT, PATCH, OPTIONS" });
                context.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });

                // manually return the status code
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("OK");
            }
            else
            {
                await this._next.Invoke(context);

                //// if the method is not options and the user is NOT authenticated, deny the request
                //if (context.Request.Method != "OPTIONS" && !context.User.Identity.IsAuthenticated)
                //{
                //    context.Response.StatusCode = 401;
                //    await context.Response.WriteAsync("Not Authenticated");
                //}
                //else
                //{
                //    // let the request to further
                //    await this._next.Invoke(context);
                //}
            }
        }
    }
}
