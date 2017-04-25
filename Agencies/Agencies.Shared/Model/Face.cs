using System.Drawing;

namespace Agencies.Shared
{
    public class Face : FaceModel
    {
        public string PhotoPath { get; set; }

        //public byte [] PhotoData { get; set; }

        public RectangleF FaceRectangle { get; set; }
    }
}