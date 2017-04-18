using UIKit;

namespace NomadCode.ClientAuth
{
    public class ClientAuthProvider
    {
        public string ProviderId { get; set; }

        public string ShortName { get; set; }

        public string SignInLabel { get; set; }

        public UIImage Icon { get; set; }

        public UIColor ButtonBackgroundColor { get; set; }

        public UIColor ButtonTextColor { get; set; }
    }
}
