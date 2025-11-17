using System.Text;

namespace Helpers;

internal static class Tools
{
    public static string ToStringProperty<T>(this T t)
    {
        if (t == null)
            return "null";

        var sb = new StringBuilder();
        sb.AppendLine(t.GetType().Name + ":");

        foreach (var prop in t.GetType().GetProperties())
        {
            var value = prop.GetValue(t);
            sb.AppendLine($"  {prop.Name} = {value}");
        }

        return sb.ToString();
    }
}
