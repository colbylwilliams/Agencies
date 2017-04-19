using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
#if __IOS__
using Xamarin.Cognitive.Face.iOS;
using Foundation;
#elif __ANDROID__
using Xamarin.Cognitive.Face.Android;
#endif

namespace Agencies.Shared
{
    public class FaceClient
    {
        static FaceClient _shared;
        public static FaceClient Shared => _shared ?? (_shared = new FaceClient ());

        public string SubscriptionKey { get; set; }

        public List<PersonGroup> Groups { get; private set; } = new List<PersonGroup> ();


#if __IOS__
        MPOFaceServiceClient client;
        MPOFaceServiceClient Client => client ?? (client = new MPOFaceServiceClient (SubscriptionKey));
#elif __ANDROID__
        FaceServiceRestClient client;
        FaceServiceRestClient Client => client ?? (client = new FaceServiceRestClient (SubscriptionKey));
#endif


        public FaceClient ()
        {
        }


        public Task<List<PersonGroup>> GetGroups ()
        {
#if __IOS__
            var tcs = new TaskCompletionSource<List<PersonGroup>> ();

            if (Groups.Count == 0)
            {
                Client.ListPersonGroupsWithCompletion ((groups, error) =>
                {
                    if (!FailTaskIfErrored (tcs, error))
                    {
                        Groups = new List<PersonGroup> (
                            groups.Select (g => new PersonGroup
                            {
                                Id = g.PersonGroupId,
                                Name = g.Name,
                                UserData = g.UserData
                            })
                        );

                        tcs.SetResult (Groups);
                    }
                }).Resume ();
            }

            return tcs.Task;
#elif __ANDROID__
            var groups = Client.ListPersonGroups ();

            return Task.FromResult (Groups);
#endif
        }


        public Task CreatePersonGroup (string personGroupId, string groupName, string userData = null)
        {
#if __IOS__
            var tcs = new TaskCompletionSource<bool> ();

            Client.CreatePersonGroupWithId (personGroupId, groupName, userData, error =>
            {
                if (!FailTaskIfErrored (tcs, error))
                {
                    var group = new PersonGroup
                    {
                        Name = groupName,
                        Id = personGroupId,
                        UserData = userData
                    };

                    Groups.Add (group);

                    tcs.SetResult (true);
                }
            }).Resume ();

            return tcs.Task;

#elif __ANDROID__
            Client.CreatePersonGroup (personGroupId, groupName, userData);

            return Task.FromResult (true);
#endif
        }


        public Task UpdatePersonGroup (PersonGroup personGroup, string groupName)
        {
#if __IOS__
            var tcs = new TaskCompletionSource<bool> ();

            Client.UpdatePersonGroupWithPersonGroupId (personGroup.Id, groupName, personGroup.UserData, error =>
            {
                if (!FailTaskIfErrored (tcs, error))
                {
                    personGroup.Name = groupName;

                    tcs.SetResult (true);
                }
            }).Resume ();

            return tcs.Task;

#elif __ANDROID__
            Client.CreatePersonGroup (personGroup.Id, groupName, personGroup.UserData);

            personGroup.Name = groupName;

            return Task.FromResult (true);
#endif
        }


        public Task TrainGroup (PersonGroup personGroup)
        {
#if __IOS__
            var tcs = new TaskCompletionSource<bool> ();

            Client.TrainPersonGroupWithPersonGroupId (personGroup.Id, error =>
            {
                if (!FailTaskIfErrored (tcs, error))
                {
                    tcs.SetResult (true);
                }
            }).Resume ();

            return tcs.Task;
#elif __ANDROID__
            return Task.FromResult (true);
#endif
        }


#if __IOS__
        bool FailTaskIfErrored<T> (TaskCompletionSource<T> tcs, NSError error, T result = default (T))
        {
            if (error != null)
            {
                tcs.SetException (new Exception (error.Description));
                tcs.SetResult (result);

                return true;
            }

            return false;
        }
#endif
    }
}