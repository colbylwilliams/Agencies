namespace Agencies.iOS
{
    public interface IHandleChildSelection<TSelection>
    {
        void HandleChildSelection (TSelection selection);
    }
}