namespace Agencies.Shared.Extensions
{
    public static class TypeExtensions
    {
        public static T ToType<T> (this object obj)
        {
            return (T)obj;
        }


        public static T ToType<T> (this object obj, T toType)
        {
            return (T)obj;
        }
    }
}