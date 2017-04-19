using System;
using System.Collections.Generic;
#if __IOS__
using Cognitive.Face.iOS;
#elif __ANDROID__
//using Cognitive.Face.Android;
#endif

namespace Agencies.Shared
{
    public class FaceClient
    {
        static FaceClient _shared;
        public static FaceClient Shared => _shared ?? (_shared = new FaceClient ());

        public string SubscriptionKey { get; set; }

        public List<PersonGroup> Groups { get; set; } = new List<PersonGroup> ();

        public FaceClient ()
        {

        }
    }
}