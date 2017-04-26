using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agencies.iOS;
using Agencies.iOS.Extensions;
using Foundation;
using UIKit;
using Xamarin.Cognitive.Face.iOS;

namespace Agencies.Shared
{
    public partial class FaceClient
    {
        MPOFaceServiceClient client;
        MPOFaceServiceClient Client => client ?? (client = new MPOFaceServiceClient (SubscriptionKey));

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
                    var tcs = new TaskCompletionSource<List<PersonGroup>> ();

                    Client.ListPersonGroupsWithCompletion ((groups, error) =>
                    {
                        tcs.FailTaskIfErrored (error.ToException ());

                        Groups = new List<PersonGroup> (
                            groups.Select (g => g.ToPersonGroup ())
                        );

                        tcs.SetResult (Groups);
                    }).Resume ();

                    return tcs.Task;
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
                var tcs = new TaskCompletionSource<PersonGroup> ();

                var personGroupId = Guid.NewGuid ().ToString ();

                Client.CreatePersonGroupWithId (personGroupId, groupName, userData, error =>
                {
                    tcs.FailTaskIfErrored (error.ToException ());

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
                var tcs = new TaskCompletionSource<bool> ();

                Client.UpdatePersonGroupWithPersonGroupId (personGroup.Id, groupName, userData, error =>
                {
                    tcs.FailTaskIfErrored (error.ToException ());

                    personGroup.Name = groupName;
                    personGroup.UserData = userData;

                    tcs.SetResult (true);
                }).Resume ();

                return tcs.Task;
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
                var tcs = new TaskCompletionSource<bool> ();

                Client.TrainPersonGroupWithPersonGroupId (personGroup.Id, error =>
                {
                    tcs.FailTaskIfErrored (error.ToException ());

                    tcs.SetResult (true);

                }).Resume ();

                return tcs.Task;
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        #endregion


        #region Person


        public Task<List<Person>> GetPeopleForGroup (PersonGroup group)
        {
            try
            {
                var tcs = new TaskCompletionSource<List<Person>> ();

                Client.ListPersonsWithPersonGroupId (group.Id, (groupPeople, error) =>
                {
                    tcs.FailTaskIfErrored (error.ToException ());

                    var people = new List<Person> (
                        groupPeople.Select (p => p.ToPerson ())
                    );

                    group.People.AddRange (people);

                    tcs.SetResult (people);
                }).Resume ();

                return tcs.Task;
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        public Task<Person> CreatePerson (string personName, PersonGroup group, string userData = null)
        {
            try
            {
                var tcs = new TaskCompletionSource<Person> ();

                Client.CreatePersonWithPersonGroupId (group.Id, personName, userData, (result, error) =>
                {
                    tcs.FailTaskIfErrored (error.ToException ());
                    tcs.FailTaskByCondition (string.IsNullOrEmpty (result.PersonId), "CreatePersonResult returned invalid person Id");

                    var person = new Person
                    {
                        Name = personName,
                        Id = result.PersonId,
                        UserData = userData
                    };

                    group.People.Add (person);

                    tcs.SetResult (person);
                }).Resume ();

                return tcs.Task;
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
                var tcs = new TaskCompletionSource<bool> ();

                Client.UpdatePersonWithPersonGroupId (group.Id, person.Id, personName, userData, error =>
                {
                    tcs.FailTaskIfErrored (error.ToException ());

                    person.Name = personName;
                    person.UserData = userData;

                    tcs.SetResult (true);
                }).Resume ();

                return tcs.Task;
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        public Task DeletePerson (Person person, PersonGroup group)
        {
            try
            {
                var tcs = new TaskCompletionSource<bool> ();

                Client.DeletePersonWithPersonGroupId (group.Id, person.Id, error =>
                {
                    tcs.FailTaskIfErrored (error.ToException ());

                    if (group.People.Contains (person))
                    {
                        group.People.Remove (person);
                    }

                    tcs.SetResult (true);
                }).Resume ();

                return tcs.Task;
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        #endregion


        #region Face


        public Task<List<Face>> GetFacesForPerson (Person person, PersonGroup group)
        {
            try
            {
                person.Faces.Clear ();

                if (person.FaceIds != null)
                {
                    var tcs = new TaskCompletionSource<List<Face>> ();

                    foreach (var faceId in person.FaceIds)
                    {
                        Client.GetPersonFaceWithPersonGroupId (group.Id, person.Id, faceId, (mpoFace, error) =>
                        {
                            tcs.FailTaskIfErrored (error.ToException ());

                            var face = mpoFace.ToFace ();

                            person.Faces.Add (face);

                            if (person.Faces.Count == person.FaceIds.Count)
                            {
                                tcs.SetResult (person.Faces);
                            }
                        }).Resume ();
                    }

                    return tcs.Task;
                }

                return Task.FromResult (default (List<Face>));
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        public Task<List<Face>> DetectFacesInPhoto (UIImage photo)
        {
            try
            {
                List<Face> faces = new List<Face> ();
                var tcs = new TaskCompletionSource<List<Face>> ();

                using (var jpgData = photo.AsJPEG (.8f))
                {
                    Client.DetectWithData (jpgData, true, true, new NSObject [0], (detectedFaces, error) =>
                    {
                        tcs.FailTaskIfErrored (error.ToException ());

                        foreach (var detectedFace in detectedFaces)
                        {
                            var face = detectedFace.ToFace ();
                            faces.Add (face);

                            using (var croppedImage = photo.Crop (face.FaceRectangle))
                            {
                                //save to disk
                                using (var data = croppedImage.AsJPEG ())
                                {
                                    data.Save (face.PhotoPath, true);
                                }
                            }
                        }

                        tcs.SetResult (faces);
                    }).Resume ();
                }

                return tcs.Task;
            }
            catch (Exception ex)
            {
                Log.Error (ex.Message);
                throw;
            }
        }


        public Task AddFaceForPerson (Person person, PersonGroup group, Face face, UIImage photo, string userData = null)
        {
            try
            {
                var tcs = new TaskCompletionSource<bool> ();
                var faceRect = face.FaceRectangle.ToMPOFaceRect ();

                using (var jpgData = photo.AsJPEG (.8f))
                {
                    Client.AddPersonFaceWithPersonGroupId (group.Id, person.Id, jpgData, userData, faceRect, (result, error) =>
                    {
                        tcs.FailTaskIfErrored (error.ToException ());
                        tcs.FailTaskByCondition (string.IsNullOrEmpty (result.PersistedFaceId), "AddPersistedFaceResult returned invalid face Id");

                        face.Id = result.PersistedFaceId;

                        person.Faces.Add (face);

                        tcs.SetResult (true);
                    }).Resume ();
                }

                return tcs.Task;
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