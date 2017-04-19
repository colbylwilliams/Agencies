using System.Collections.Generic;

namespace Agencies.Shared
{
    public class PersonGroup : FaceModel
    {
        public List<Person> People { get; set; }

        public string UserData { get; set; }
    }
}