using Android.App;
using Android.Widget;
using Android.OS;
using Com.Microsoft.Projectoxford.Face;

namespace Agencies.Droid
{
	[Activity (Label = "Agencies", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			FaceServiceRestClient client = new FaceServiceRestClient (Keys.CognitiveServices.FaceApi.SubscriptionKey);
		}
	}
}