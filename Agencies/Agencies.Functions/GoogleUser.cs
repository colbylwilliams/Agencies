using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace Agencies.Functions
{
	public class GoogleUser
	{
		[JsonIgnore]
		public string Provider => "google";

		[JsonProperty ("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty ("refresh_token")]
		public string RefreshToken { get; set; }

		[JsonProperty ("expires_on")]
		public DateTimeOffset? AccessTokenExpiration { get; set; }

		[JsonProperty ("id_token")]
		public string IdToken { get; set; }

		[JsonProperty ("provider_name")]
		public string ProviderName { get; set; }

		[JsonProperty ("user_claims", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<Claim> UserClaims { get; set; }

		[JsonProperty ("user_id")]
		public string UserId { get; set; }




		[JsonIgnore]
		public string NameIdentifier => UserClaims?.FirstOrDefault (c => string.Compare (c.Type, ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase) == 0)?.Value;

		[JsonIgnore]
		public string EmailAddress => UserClaims?.FirstOrDefault (c => string.Compare (c.Type, ClaimTypes.Email, StringComparison.OrdinalIgnoreCase) == 0)?.Value;

		[JsonIgnore]
		public bool EmailVerified => string.Compare (UserClaims?.FirstOrDefault (c => string.Compare (c.Type, "email_verified", StringComparison.OrdinalIgnoreCase) == 0)?.Value, "true", StringComparison.OrdinalIgnoreCase) == 0;

		[JsonIgnore]
		public string AtHash => UserClaims?.FirstOrDefault (c => string.Compare (c.Type, "at_hash", StringComparison.OrdinalIgnoreCase) == 0)?.Value;

		[JsonIgnore]
		public string Issuer => UserClaims?.FirstOrDefault (c => string.Compare (c.Type, "iss", StringComparison.OrdinalIgnoreCase) == 0)?.Value;

		[JsonIgnore]
		public string IssuedAt => UserClaims?.FirstOrDefault (c => string.Compare (c.Type, "iat", StringComparison.OrdinalIgnoreCase) == 0)?.Value;

		[JsonIgnore]
		public string Expires => UserClaims?.FirstOrDefault (c => string.Compare (c.Type, "exp", StringComparison.OrdinalIgnoreCase) == 0)?.Value;

		[JsonIgnore]
		public string Name => UserClaims?.FirstOrDefault (c => string.Compare (c.Type, "name", StringComparison.OrdinalIgnoreCase) == 0)?.Value;

		[JsonIgnore]
		public string Picture => UserClaims?.FirstOrDefault (c => string.Compare (c.Type, "picture", StringComparison.OrdinalIgnoreCase) == 0)?.Value;

		[JsonIgnore]
		public string GivenName => UserClaims?.FirstOrDefault (c => string.Compare (c.Type, ClaimTypes.GivenName, StringComparison.OrdinalIgnoreCase) == 0)?.Value;

		[JsonIgnore]
		public string Surname => UserClaims?.FirstOrDefault (c => string.Compare (c.Type, ClaimTypes.Surname, StringComparison.OrdinalIgnoreCase) == 0)?.Value;

		[JsonIgnore]
		public string Locale => UserClaims?.FirstOrDefault (c => string.Compare (c.Type, "locale", StringComparison.OrdinalIgnoreCase) == 0)?.Value;


		public override string ToString ()
		{
			return string.Format ("[GoogleUser:\n\tProvider={0},\n\tAccessToken={1},\n\tRefreshToken={2},\n\tAccessTokenExpiration={3},\n\tIdToken={4},\n\tProviderName={5},\n\tUserClaims={6},\n\tUserId={7},\n\tNameIdentifier={8},\n\tEmailAddress={9},\n\tEmailVerified={10},\n\tAtHash={11},\n\tIssuer={12},\n\tIssuedAt={13},\n\tExpires={14},\n\tName={15},\n\tPicture={16},\n\tGivenName={17},\n\tSurname={18},\n\tLocale={19}]", Provider, AccessToken, RefreshToken, AccessTokenExpiration, IdToken, ProviderName, UserClaims, UserId, NameIdentifier, EmailAddress, EmailVerified, AtHash, Issuer, IssuedAt, Expires, Name, Picture, GivenName, Surname, Locale);
		}
	}
}
