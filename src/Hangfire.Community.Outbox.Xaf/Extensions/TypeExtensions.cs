namespace Hangfire.Community.Outbox.Xaf.Extensions;

public static class TypeExtensions
{
    public static string GetFriendlyName(this Type type)
    {
        return $"{type.FullName}, {type.Assembly.GetName().Name}";
    }
}