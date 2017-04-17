using Microsoft.Azure.Mobile.Server.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;

namespace Agencies.AppService.Controllers
{
	[MobileAppController]
	public class GetFaceTokenController : ApiController
	{
		const string faceApiSubscriptionKey = "MS_FaceApiSubscriptionKey";

		static string _faceApiSubscription;
		static string faceApiSubscription => _faceApiSubscription ?? (_faceApiSubscription = ConfigurationManager.AppSettings[faceApiSubscriptionKey]);

		// GET api/getFaceToken
#if !DEBUG
		[Authorize]
#endif
		public string Get()
		{
			return faceApiSubscription;
		}
	}
}