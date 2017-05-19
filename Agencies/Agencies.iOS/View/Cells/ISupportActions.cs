using System;
using Foundation;

namespace Agencies.iOS
{
	public interface ISupportGestureAction
	{
		void AttachAction (Action<NSObject> action);

		void DetachAction ();
	}
}