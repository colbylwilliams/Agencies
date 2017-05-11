using System;
using UIKit;

namespace Agencies.iOS
{
    public partial class DetectionResultsTableViewCell : UITableViewCell
    {
        public UILabel Title => TitleLabel;
        public UILabel Size => SizeLabel;
        public UILabel Age => AgeLabel;
        public UILabel Smile => SmileLabel;
        public UILabel Gender => GenderLabel;
        public UILabel HeadPose => HeadPoseLabel;
        public UILabel FacialHair => FacialHairLabel;
        public UILabel Hair => HairLabel;
        public UILabel Makeup => MakeupLabel;
        public UILabel Glasses => GlassesLabel;
        public UILabel Emotion => EmotionLabel;
        public UILabel Accessories => AccessoriesLabel;
        public UILabel Occlusion => OcclusionLabel;
        public UILabel Blur => BlurLabel;
        public UILabel Noise => NoiseLabel;
        public UILabel Exposure => ExposureLabel;

        public override UIImageView ImageView => FaceImageView;

        public DetectionResultsTableViewCell (IntPtr handle) : base (handle)
        {
        }
    }
}