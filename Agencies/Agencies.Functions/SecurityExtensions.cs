using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Agencies.Functions
{
	public static class SecurityExtensions
	{
		// https://github.com/Azure/azure-mobile-apps-net-server/wiki/Understanding-User-Ids
		public static string UniqueIdentifier (this IPrincipal user)
		{
			if (user is ClaimsPrincipal principal)
			{
				if (principal.Identity is ClaimsIdentity identity)
				{
					return identity.UniqueIdentifier ();
				}
			}

			return null;
		}


		public static string UniqueIdentifier (this ClaimsIdentity identity)
		{
			if (identity != null)
			{
				var stableSid = string.Empty;

				var ver = identity.FindFirst ("ver")?.Value;

				// the NameIdentifier claim is not stable.
				if (string.Compare (ver, "3", StringComparison.OrdinalIgnoreCase) == 0)
				{
					// the NameIdentifier claim is not stable.
					stableSid = identity.FindFirst ("stable_sid")?.Value;
				}
				else if (string.Compare (ver, "4", StringComparison.OrdinalIgnoreCase) == 0)
				{
					// the NameIdentifier claim is stable.
					stableSid = identity.FindFirst (ClaimTypes.NameIdentifier).Value;
				}

				var provider = identity.FindFirst ("http://schemas.microsoft.com/identity/claims/identityprovider")?.Value;

				if (string.IsNullOrEmpty (stableSid) || string.IsNullOrEmpty (provider))
				{
					return null;
				}

				return $"{provider}|{stableSid}";
			}

			return null;
		}
	}
}
