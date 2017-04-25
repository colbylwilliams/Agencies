using System.Collections.Generic;

namespace Agencies.Shared
{
    public partial class FaceClient
    {
        static FaceClient _shared;
        public static FaceClient Shared => _shared ?? (_shared = new FaceClient ());

        public string SubscriptionKey { get; set; }

        public List<PersonGroup> Groups { get; private set; } = new List<PersonGroup> ();


    }
}