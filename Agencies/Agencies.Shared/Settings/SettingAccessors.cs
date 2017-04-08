﻿/* This file was generated by Settings Studio
 *
 * Copyright © 2015 Colby Williams. All Rights Reserved.
 */

namespace SettingsStudio
{
	public static partial class Settings
	{
		#region Visible Settings


		public static string VersionNumber
		{
			get => StringForKey(SettingsKeys.VersionNumber);
#if __ANDROID__
			set => SetSetting (SettingsKeys.VersionNumber, value);
#endif
		}


		public static string BuildNumber
		{
			get => StringForKey(SettingsKeys.BuildNumber);
#if __ANDROID__
			set => SetSetting (SettingsKeys.BuildNumber, value);
#endif
		}


		public static string GitHash => StringForKey(SettingsKeys.GitCommitHash);


		public static string UserReferenceKey
		{
			get => StringForKey(SettingsKeys.UserReferenceKey);
			set => SetSetting(SettingsKeys.UserReferenceKey, value);
		}


		#endregion


		#region Hidden Settings

		public static string ConversationId
		{
			get => StringForKey(SettingsKeys.ConversationId);
			set => SetSetting(SettingsKeys.ConversationId, value);
		}

		#endregion


		#region Debug
#if DEBUG

		public static bool UseLocalServer
		{
			get => BoolForKey(SettingsKeys.UseLocalServer);
			set => SetSetting(SettingsKeys.UseLocalServer, value);
		}


		public static bool ResetConversation
		{
			get
			{
				var reset = BoolForKey(SettingsKeys.ResetConversation);

				if (reset)
				{
					SetSetting(SettingsKeys.ResetConversation, false);
				}

				return reset;
			}
		}

#endif
		#endregion

	}
}
