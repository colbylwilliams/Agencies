using System.Collections.Generic;

namespace Agencies.Shared
{
    public class PersonGroup : FaceModel
    {
        public List<Person> People { get; set; } = new List<Person> ();

        public string UserData { get; set; }
    }
}