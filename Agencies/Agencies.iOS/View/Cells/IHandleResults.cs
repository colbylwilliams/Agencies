namespace Agencies.iOS
{
	public interface IHandleResults<TResult>
	{
		void SetResult (TResult result);
	}
}