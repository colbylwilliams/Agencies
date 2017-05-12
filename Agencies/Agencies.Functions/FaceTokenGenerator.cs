using System;
using System.Net;
using System.Net.Http;

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
			return req.CreateResponse (HttpStatusCode.OK, faceApiSubscription);
        }
    }
}