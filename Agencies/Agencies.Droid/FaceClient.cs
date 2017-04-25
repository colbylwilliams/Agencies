using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Cognitive.Face.Android;
using Java.Util;

namespace Agencies.Shared
{
    public partial class FaceClient
    {
        FaceServiceRestClient client;
        FaceServiceRestClient Client => client ?? (client = new FaceServiceRestClient (SubscriptionKey));

        public FaceClient ()
        {
        }


        #region Groups


        public Task<List<PersonGroup>> GetGroups (bool forceRefresh = false)
        {
            try
            {
                if (Groups.Count == 0 || forceRefresh)
                {
                    var groups = Client.ListPersonGroups ();

                    Groups = new List<PersonGroup> (
                        groups.Select (g => new PersonGroup
                        {
                            Id = g.PersonGroupId,
                            Name = g.Name,
                            UserData = g.UserData
                        })
                    );
                }

                return Task.FromResult (Groups);
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        public Task<PersonGroup> CreatePersonGroup (string groupName, string userData = null)
        {
            try
            {
                var personGroupId = Guid.NewGuid ().ToString ();

                Client.CreatePersonGroup (personGroupId, groupName, userData);

                var group = new PersonGroup
                {
                    Name = groupName,
                    Id = personGroupId,
                    UserData = userData
                };

                Groups.Add (group);

                return Task.FromResult (group);
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        public Task UpdatePersonGroup (PersonGroup personGroup, string groupName, string userData = null)
        {
            try
            {
                Client.UpdatePersonGroup (personGroup.Id, groupName, userData);

                personGroup.Name = groupName;
                personGroup.UserData = userData;

                return Task.FromResult (true);
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        public Task TrainGroup (PersonGroup personGroup)
        {
            try
            {
                Client.TrainPersonGroup (personGroup.Id);

                return Task.FromResult (true);
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        #endregion


        #region Person


        public Task<Person> CreatePerson (string personName, PersonGroup group, string userData = null)
        {
            try
            {
                var result = Client.CreatePerson (group.Id, personName, userData);

                var id = result.PersonId.ToString ();

                if (string.IsNullOrEmpty (id))
                {
                    throw new Exception ("CreatePersonResult returned invalid person Id");
                }

                var person = new Person
                {
                    Name = personName,
                    Id = id,
                    UserData = userData
                };

                group.People.Add (person);

                return Task.FromResult (person);
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        public Task UpdatePerson (Person person, PersonGroup group, string personName, string userData = null)
        {
            try
            {
                Client.UpdatePerson (group.Id, UUID.FromString (person.Id), personName, userData);

                person.Name = personName;
                person.UserData = userData;

                return Task.FromResult (true);
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        #endregion
    }
}