using System;
using Agencies.Shared;

namespace Agencies.iOS
{
	public partial class IdentifyResultsTableViewController : FaceResultsTableViewController<IdentifyResultTableViewCell, IdentificationResult>
	{
		public IdentifyResultsTableViewController (IntPtr handle) : base (handle)
		{
		}
	}
}