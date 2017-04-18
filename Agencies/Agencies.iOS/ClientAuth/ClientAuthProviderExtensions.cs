﻿using System;
using System.Collections.Generic;

using UIKit;

namespace NomadCode.ClientAuth
{
    public static class ClientAuthProviderExtensions
    {
        public static ClientAuthProvider FromTag (nint tag) => providers [(int)tag];

        static List<ClientAuthProvider> providers = new List<ClientAuthProvider>
        {
            new ClientAuthProvider
            {
                ButtonBackgroundColor = UIColor.White,
                ButtonTextColor = UIColor.FromWhiteAlpha(0, 0.54f),
                Icon = UIImage.FromBundle("nc_clientauth_i_google"),
                ProviderId = "foo",
                ShortName = "Google",
                SignInLabel = "Sign in with Google"
            },
            new ClientAuthProvider
            {
                ButtonBackgroundColor = UIColor.FromRGBA (59.0f / 255.0f, 89.0f / 255.0f, 152.0f / 255.0f, 1.0f),
                ButtonTextColor = UIColor.White,
                Icon = UIImage.FromBundle("nc_clientauth_i_facebook"),
                ProviderId = "foo",
                ShortName = "Facebook",
                SignInLabel = "Sign in with Facebook"
            },
            new ClientAuthProvider
            {
                ButtonBackgroundColor = UIColor.White,
                ButtonTextColor = UIColor.FromWhiteAlpha(0, 0.54f),
                Icon = UIImage.FromBundle("nc_clientauth_i_microsoft"),
                ProviderId = "foo",
                ShortName = "Microsoft",
                SignInLabel = "Sign in with Microsoft"
            },
            new ClientAuthProvider
            {
                ButtonBackgroundColor = UIColor.FromRGBA(71.0f / 255.0f, 154.0f / 255.0f, 234.0f / 255.0f, 1.0f),
                ButtonTextColor = UIColor.White,
                Icon = UIImage.FromBundle("nc_clientauth_i_twitter"),
                ProviderId = "foo",
                ShortName = "Twitter",
                SignInLabel = "Sign in with Twitter"
            }
        };
    }
}
