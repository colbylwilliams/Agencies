using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Security.Principal;
using System.Linq;
using Newtonsoft.Json;

namespace Agencies.Functions
{
	public static class FaceTokenGenerator
	{
		static string _faceApiSubscription;
		static string faceApiSubscription => _faceApiSubscription ?? (_faceApiSubscription = Environment.GetEnvironmentVariable ("MS_FaceApiSubscriptionKey"));


		[FunctionName ("GetFaceToken")]
		public static HttpResponseMessage GetFaceToken ([HttpTrigger (AuthorizationLevel.Anonymous, "get", Route = "tokens/face")]HttpRequestMessage req, TraceWriter log)
		{
			if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
			{
				log.Info ("Not authenticated");

				return req.CreateResponse (HttpStatusCode.Unauthorized);
			}

			log.Info ("Authenticated");

			if (Thread.CurrentPrincipal is ClaimsPrincipal principal)
			{
				log.Info ($"principal:");

				foreach (var claim in principal.Claims)
				{
					log.Info ($"           claim: {claim.Type} = {claim.Value}");
				}

				foreach (var identity in principal.Identities)
				{
					log.Info ($"identity: {identity.Name}");

					foreach (var claim in identity.Claims)
					{
						log.Info ($"          claim: {claim.Type} = {claim.Value}");
					}


					using (var client = new HttpClient ())
					{
						client.DefaultRequestHeaders.Add ("x-zumo-auth", req.Headers.GetValues ("x-zumo-auth").First ());

						var me = client.GetStringAsync ("https://digital-agencies-functions.azurewebsites.net/.auth/me").Result;

						log.Info ($"me: {me}");

						var googleUser = JsonConvert.DeserializeObject<GoogleUser> (me);

						log.Info ($"googleUser {googleUser}");
					}
				}


				//if (principal.Identity is ClaimsIdentity identity)
				//{
				//	var userId = identity.UniqueIdentifier ();


				//}
			}

			log.Info ($"current:");

			foreach (var claim in ClaimsPrincipal.Current.Claims)
			{
				log.Info ($"          claim: {claim.Type} = {claim.Value}");
			}



			if (string.IsNullOrEmpty (faceApiSubscription))
			{
				return req.CreateErrorResponse (HttpStatusCode.NotFound, "Unable to find Face token on server");
			}

			return req.CreateResponse (HttpStatusCode.OK, faceApiSubscription);
		}
	}
}