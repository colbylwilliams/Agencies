using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Newtonsoft.Json;

using NomadCode.Auth;

namespace Agencies.Functions
{
    public static class AuthUserConfigProvider
    {
        [Authorize]
        [FunctionName("GetUserConfig")]
        public static async Task<HttpResponseMessage> GetUserConfig ([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/config")]HttpRequestMessage req, TraceWriter log)
        {
            if (Thread.CurrentPrincipal is ClaimsPrincipal principal)
            {
                log.Info($"principal:");

                if (principal.Identity is ClaimsIdentity identity)
                {
                    var userId = identity.UniqueIdentifier();

                    log.Info($"userId = {userId}");

                    using (var client = new HttpClient())
                    {
                        client.ConfigureClientForUserDetails(identity, req);

                        log.Info($"client.BaseAddress = {client.BaseAddress}");

                        try
                        {
                            var me = await client.GetStringAsync(".auth/me");

                            // TODO: Check for provider
                            var googleUser = JsonConvert.DeserializeObject<GoogleUser>(me.Trim(new Char[] { '[', ']' }));

                            return req.CreateResponse(System.Net.HttpStatusCode.OK, googleUser.GetAuthUserConfig(userId));
                        }
                        catch (Exception ex)
                        {
                            log.Error("Could not get user details", ex);
                            throw;
                        }
                    }
                }
            }

            return req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
        }
    }
}