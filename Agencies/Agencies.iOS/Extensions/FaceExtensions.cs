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
                FaceIds = person.FaceIds?.Select (fid => fid.ToString ()).ToList ()
            };
        }


        public static Face ToFace (this MPOFace mpoFace)
        {
            var rect = new RectangleF (mpoFace.FaceRectangle.Left.FloatValue,
                                       mpoFace.FaceRectangle.Top.FloatValue,
                                       mpoFace.FaceRectangle.Width.FloatValue,
                                       mpoFace.FaceRectangle.Height.FloatValue);

            var file = Path.Combine (docsDir, $"face-{mpoFace.FaceId}.jpg");

            return new Face
            {
                Id = mpoFace.FaceId,
                PhotoPath = file,
                FaceRectangle = rect,
                //TODO: WHAT ELSE GOES HERE???
            };
        }


        public static Face ToFace (this MPOPersonFace mpoFace)
        {
            var file = Path.Combine (docsDir, $"face-{mpoFace.FaceId}.jpg");

            return new Face
            {
                Id = mpoFace.FaceId,
                PhotoPath = file,
                //UserData = mpoFace.UserData,
                //FaceRectangle = rect
            };
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