using System;
using System.Collections.Generic;
using Cognitive.Face.iOS;

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