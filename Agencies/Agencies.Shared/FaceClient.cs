using System;
using System.Collections.Generic;
using Cognitive.Face.iOS;

namespace Agencies.Shared
{
    public class FaceClient
    {
        static FaceClient instance;

        public static FaceClient Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new FaceClient ();
                }

                return instance;
            }
        }

        public List<MPOPersonGroup> Groups { get; set; } = new List<MPOPersonGroup> ();

        public FaceClient ()
        {
        }
    }
}