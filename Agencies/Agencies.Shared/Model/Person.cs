﻿using System.Collections.Generic;

namespace Agencies.Shared
{
    public class Person : FaceModel
    {
        public List<string> FaceIds { get; set; } = new List<string> ();

        public List<Face> Faces { get; set; } = new List<Face> ();
    }
}