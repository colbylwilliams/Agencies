using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace Agencies.Functions
{
    public static class FaceTokenGenerator
    {
		static string _faceApiSubscription;
		static string faceApiSubscription => _faceApiSubscription ?? (_faceApiSubscription = Environment.GetEnvironmentVariable ("MS_FaceApiSubscriptionKey"));


		[FunctionName ("GetFaceToken")]
        public static HttpResponseMessage GetFaceToken ([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "tokens/face")]HttpRequestMessage req, TraceWriter log)
        {
            if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                log.Info("Not authenticated");

                return req.CreateResponse(HttpStatusCode.Unauthorized);
            }

            log.Info("Authenticated");

            if ((Thread.CurrentPrincipal as ClaimsPrincipal)?.Identity is ClaimsIdentity identity)
            {
                foreach (var claim in identity.Claims)
                {
                    log.Info($"claim: {claim.Type} = {claim.Value}");
                }
            }

            if (string.IsNullOrEmpty(faceApiSubscription))
            {
                return req.CreateErrorResponse(HttpStatusCode.NotFound, "Unable to find Face token on server");
            }

            return req.CreateResponse (HttpStatusCode.OK, faceApiSubscription);
        }
    }
}