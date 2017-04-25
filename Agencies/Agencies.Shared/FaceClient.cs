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


        public Task<List<PersonGroup>> GetGroups (bool forceRefresh = false)
        {
            try
            {
#if __IOS__
                if (Groups.Count == 0 || forceRefresh)
                {
                    var tcs = new TaskCompletionSource<List<PersonGroup>> ();

                    Client.ListPersonGroupsWithCompletion ((groups, error) =>
                    {
                        FailTaskIfErrored (tcs, error);

                        Groups = new List<PersonGroup> (
                            groups.Select (g => new PersonGroup
                            {
                                Id = g.PersonGroupId,
                                Name = g.Name
                                // Dealing with a binding issue on UserData - since it's populated via a JSON NSDict, the value will be NSNull, but we're sending a Selector to it assuming it's an NSString
                                //UserData = g.UserData
                            })
                        );

                        tcs.SetResult (Groups);
                    }).Resume ();

                    return tcs.Task;
                }

                return Task.FromResult (Groups);
#elif __ANDROID__
            var groups = Client.ListPersonGroups ();

            return Task.FromResult (Groups);
#endif
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
#if __IOS__
                var tcs = new TaskCompletionSource<PersonGroup> ();

                var personGroupId = Guid.NewGuid ().ToString ();

                Client.CreatePersonGroupWithId (personGroupId, groupName, userData, error =>
                {
                    FailTaskIfErrored (tcs, error);

                    var group = new PersonGroup
                    {
                        Name = groupName,
                        Id = personGroupId,
                        UserData = userData
                    };

                    Groups.Add (group);

                    tcs.SetResult (group);
                }).Resume ();

                return tcs.Task;

#elif __ANDROID__
	            Client.CreatePersonGroup (personGroupId, groupName, userData);

	            return Task.FromResult (null);
#endif
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
#if __IOS__
                var tcs = new TaskCompletionSource<bool> ();

                Client.UpdatePersonGroupWithPersonGroupId (personGroup.Id, groupName, userData, error =>
                {
                    FailTaskIfErrored (tcs, error);

                    personGroup.Name = groupName;
                    personGroup.UserData = userData;

                    tcs.SetResult (true);
                }).Resume ();

                return tcs.Task;

#elif __ANDROID__
	            Client.CreatePersonGroup (personGroup.Id, groupName, personGroup.UserData);

	            personGroup.Name = groupName;

	            return Task.FromResult (true);
#endif
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
#if __IOS__
                var tcs = new TaskCompletionSource<bool> ();

                Client.TrainPersonGroupWithPersonGroupId (personGroup.Id, error =>
                {
                    FailTaskIfErrored (tcs, error);

                    tcs.SetResult (true);

                }).Resume ();

                return tcs.Task;
#elif __ANDROID__
                return Task.FromResult (true);
#endif
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


#if __IOS__
        void FailTaskIfErrored<T> (TaskCompletionSource<T> tcs, NSError error)
        {
            if (error != null)
            {
                tcs.SetException (new Exception (error.Description));
            }
        }
#endif
    }
}