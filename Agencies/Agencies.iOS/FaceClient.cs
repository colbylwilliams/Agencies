using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agencies.iOS;
using Foundation;
using UIKit;
using CoreGraphics;
using Xamarin.Cognitive.Face.iOS;
using System.IO;
using System.Drawing;

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
                    FailTaskIfErrored (tcs, error);

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
                    FailTaskIfErrored (tcs, error);

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


        public Task<Person> CreatePerson (string personName, PersonGroup group, string userData = null)
        {
            try
            {
                var tcs = new TaskCompletionSource<Person> ();

                Client.CreatePersonWithPersonGroupId (group.Id, personName, userData, (result, error) =>
                {
                    FailTaskIfErrored (tcs, error);
                    FailTaskByCondition (tcs, string.IsNullOrEmpty (result.PersonId), "CreatePersonResult returned invalid person Id");

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
                    FailTaskIfErrored (tcs, error);

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
                        FailTaskIfErrored (tcs, error);

                        foreach (var detectedFace in detectedFaces)
                        {
                            var rect = new RectangleF (detectedFace.FaceRectangle.Left.FloatValue,
                                                  detectedFace.FaceRectangle.Top.FloatValue,
                                                  detectedFace.FaceRectangle.Width.FloatValue,
                                                  detectedFace.FaceRectangle.Height.FloatValue);

                            var croppedImage = photo.Crop (rect);
                            var documentsDirectory = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
                            var file = Path.Combine (documentsDirectory, $"face-{detectedFace.FaceId}.jpg");

                            //save to disk
                            using (var data = croppedImage.AsJPEG ())
                            {
                                data.Save (file, true);
                            }

                            var face = new Face
                            {
                                Id = detectedFace.FaceId,
                                PhotoPath = file,
                                FaceRectangle = rect
                                //TODO: WHAT ELSE GOES HERE???
                            };

                            faces.Add (face);
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

                var faceRect = new MPOFaceRectangle
                {
                    Left = face.FaceRectangle.Left,
                    Top = face.FaceRectangle.Top,
                    Width = face.FaceRectangle.Width,
                    Height = face.FaceRectangle.Height
                };

                using (var jpgData = photo.AsJPEG (.8f))
                {
                    Client.AddPersonFaceWithPersonGroupId (group.Id, person.Id, jpgData, userData, faceRect, (result, error) =>
                    {
                        FailTaskIfErrored (tcs, error);
                        FailTaskByCondition (tcs, string.IsNullOrEmpty (result.PersistedFaceId), "AddPersistedFaceResult returned invalid face Id");

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


        void FailTaskIfErrored<T> (TaskCompletionSource<T> tcs, NSError error)
        {
            if (error != null)
            {
                tcs.SetException (new Exception (error.Description));
            }
        }


        void FailTaskByCondition<T> (TaskCompletionSource<T> tcs, bool failureCondition, string error)
        {
            if (failureCondition)
            {
                tcs.SetException (new Exception (error));
            }
        }
    }
}