using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Agencies.Shared;
using Xamarin.Cognitive.Face.iOS;

namespace Agencies.iOS.Extensions
{
    public static class FaceExtensions
    {
        static string docsDir;

        static FaceExtensions ()
        {
            docsDir = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
        }


        public static PersonGroup ToPersonGroup (this MPOPersonGroup group)
        {
            return new PersonGroup
            {
                Id = group.PersonGroupId,
                Name = group.Name
                // Dealing with a binding issue on UserData - since it's populated via a JSON NSDict, the value will be NSNull, but we're sending a Selector to it assuming it's an NSString
                //UserData = g.UserData
            };
        }


        public static Person ToPerson (this MPOPerson person)
        {
            return new Person
            {
                Id = person.PersonId,
                Name = person.Name,
                //UserData = p.UserData,
                FaceIds = person.PersistedFaceIds?.ToList ()
            };
        }


        public static Face ToFace (this MPOFace mpoFace)
        {
            var rect = new RectangleF (mpoFace.FaceRectangle.Left.FloatValue,
                                       mpoFace.FaceRectangle.Top.FloatValue,
                                       mpoFace.FaceRectangle.Width.FloatValue,
                                       mpoFace.FaceRectangle.Height.FloatValue);

            var face = new Face
            {
                Id = mpoFace.FaceId,
                FaceRectangle = rect
            };

            face.UpdatePhotoPath ();

            return face;
        }


        public static Face ToFace (this MPOPersonFace mpoFace)
        {
            var face = new Face
            {
                Id = mpoFace.PersistedFaceId
                //UserData = mpoFace.UserData,
                //FaceRectangle = rect
            };

            face.UpdatePhotoPath ();

            return face;
        }


        public static void UpdatePhotoPath (this Face face)
        {
            var filePath = Path.Combine (docsDir, face.FileName);
            face.PhotoPath = filePath;
        }


        public static MPOFaceRectangle ToMPOFaceRect (this RectangleF rect)
        {
            return new MPOFaceRectangle
            {
                Left = rect.Left,
                Top = rect.Top,
                Width = rect.Width,
                Height = rect.Height
            };
        }
    }
}