using System;
using Agencies.Shared;

namespace Agencies.iOS
{
	public partial class SimilarFaceResultsTableViewController : FaceResultsTableViewController<SimilarFaceResultTableViewCell, SimilarFaceResult>
	{
		public SimilarFaceResultsTableViewController (IntPtr handle) : base (handle)
		{
		}
	}
}