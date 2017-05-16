using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace Agencies.Functions
{
	public static class FaceTokenGenerator
	{
		static string _faceApiSubscription;
		static string faceApiSubscription => _faceApiSubscription ?? (_faceApiSubscription = Environment.GetEnvironmentVariable ("MS_FaceApiSubscriptionKey"));

		[Authorize]
		[FunctionName ("GetFaceToken")]
		public static HttpResponseMessage GetFaceToken ([HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "tokens/face")]HttpRequestMessage req, TraceWriter log)
		{
			if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
			{
				log.Info ("Not authenticated");

				return req.CreateResponse (HttpStatusCode.Unauthorized);
			}

			if (string.IsNullOrEmpty (faceApiSubscription))
			{
				return req.CreateErrorResponse (HttpStatusCode.NotFound, "Unable to find Face token on server");
			}

			return req.CreateResponse (HttpStatusCode.OK, faceApiSubscription);
		}
	}
}