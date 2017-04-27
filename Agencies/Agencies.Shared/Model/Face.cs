using System.Drawing;

namespace Agencies.Shared
{
    public class Face : FaceModel
    {
        public const string PhotoPathTemplate = "face-{0}.jpg";

        public string PhotoPath { get; set; }

        public string FileName
        {
            get
            {
                return string.Format (PhotoPathTemplate, Id);
            }
        }

        public RectangleF FaceRectangle { get; set; }
    }
}