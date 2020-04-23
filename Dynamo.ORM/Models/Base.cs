namespace Dynamo.ORM.Models
{
    public class Base
    {
        internal static string GetPropertyReference(string propertyName) => $"#{char.ToLowerInvariant(propertyName[0])}{propertyName.Substring(1)}";
    }
}