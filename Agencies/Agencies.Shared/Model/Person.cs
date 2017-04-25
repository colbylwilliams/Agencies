using System.Collections.Generic;

namespace Agencies.Shared
{
    public class Person : FaceModel
    {
        public List<Face> Faces { get; set; } = new List<Face> ();

        public string UserData { get; set; }
    }
}