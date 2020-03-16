using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.IO;
using System.Text;

namespace PlatformAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class VehicleController : ControllerBase
    {
        /// <summary>
        /// The web API will accept only tokens 1) for users, 2) that have the `Vehicle.Command` scope for
        /// this API. See
        /// https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-protected-web-api-verification-scope-app-roles
        /// </summary>
        const string scopeRequiredByAPI = "user_impersonation";

        private static readonly string[] VehicleBrands = new[]
        {
            "Benz", "Spark", "Porsche", "Lambo"
        };

        private readonly ILogger<VehicleController> _logger;
        private readonly Services.IoTService _service;

        public VehicleController(ILogger<VehicleController> logger, Services.IoTService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public IEnumerable<Vehicle> Get()
        {
            try
            {
                HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByAPI);
            }
            catch (AuthenticationException e)
            {
                HttpContext.Response.StatusCode = 401;
                HttpContext.Response.Headers.Add("Exception", e.Message);
                return null;
            }

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new Vehicle
            {
                Date = DateTime.Now.AddDays(index),
                Speed = rng.Next(0, 155),
                Brand = VehicleBrands[rng.Next(VehicleBrands.Length)]
            })
            .ToArray();
        }

        [HttpPost(Name = "background")]
        public void Post(BackgroundImage content)
        {
            try
            {
                HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByAPI);
            }
            catch (AuthenticationException e)
            {
                _logger.LogError(e.Message);
                HttpContext.Response.StatusCode = 401;
                HttpContext.Response.Headers.Add("Exception", e.Message);
            }

            try
            {
                _logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(content));
                _service.SendMessage(content);
                HttpContext.Response.StatusCode = 200;

            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                _logger.LogError(e.Message);
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.Headers.Add("Exception", e.Message);
            }
            
        }
    }

    public static class Extensions
    {
        /// <summary>
        /// When applied to a <see cref="HttpContext"/>, verifies that the user authenticated in the
        /// web API has any of the accepted scopes.
        /// If the authenticated user doesn't have any of these <paramref name="acceptedScopes"/>, the
        /// method throws an HTTP Unauthorized error with a message noting which scopes are expected in the token.
        /// </summary>
        /// <param name="acceptedScopes">Scopes accepted by this API</param>
        /// <exception cref="HttpRequestException"/> with a <see cref="HttpResponse.StatusCode"/> set to 
        /// <see cref="HttpStatusCode.Unauthorized"/>
        public static void VerifyUserHasAnyAcceptedScope(this HttpContext context,
                                                        params string[] acceptedScopes)
        {
            if (acceptedScopes == null)
            {
                throw new ArgumentNullException(nameof(acceptedScopes));
            }
            Claim scopeClaim = context?.User
                                        ?.FindFirst("http://schemas.microsoft.com/identity/claims/scope");
            if (scopeClaim == null || !scopeClaim.Value.Split(' ').Intersect(acceptedScopes).Any())
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                string message = $"The 'scope' claim does not contain scopes '{string.Join(",", acceptedScopes)}' or was not found";
                throw new AuthenticationException(message);
            }
        }
    }
}
