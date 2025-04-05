using System.Collections;
using System.Linq.Expressions;
using System.Text;
using Socolin.TestUtils.AutoFillTestObjects.Utils;

namespace Socolin.TestUtils.AutoFillTestObjects;

public static class AutoFill<T> where T : class, new()
{
    public static T One(AutoFillFlags flags = AutoFillFlags.None, AutoFillSettings settings = null, Expression<Func<T, object>> ignoring = null)
    {
        var autoFillSettings = settings ?? new AutoFillSettings();
        var context = new AutoFillContext
        {
            Flags = flags,
            IntValue = autoFillSettings.StartIntValue,
            Settings = autoFillSettings,
            IgnoredMembers = BuildIgnoredProperties(ignoring),
        };

        return (T) CreateElement(typeof(T), context);
    }

    private static HashSet<string> BuildIgnoredProperties(Expression<Func<T, object>> ignoring)
    {
        var ignoredMembers = new HashSet<string>();

        var arguments = (ignoring?.Body as NewExpression)?.Arguments;
        if (arguments == null)
            return ignoredMembers;

        foreach (var argument in arguments)
        {
            var membersChain = new List<string>();
            BuildMemberChain(argument, membersChain);
            membersChain.Reverse();
            ignoredMembers.Add(string.Join(".", membersChain));
        }

        return ignoredMembers;
    }

    private static void BuildMemberChain(Expression expression, List<string> membersChain)
    {
        if (expression is MemberExpression memberExpression)
        {
            membersChain.Add(memberExpression.Member.Name);
            BuildMemberChain(memberExpression.Expression, membersChain);
        }
    }

    private static object CreateElement(Type type, AutoFillContext context)
    {
        if (type.IsInterface)
            return null;
        var element = Activator.CreateInstance(type);

        if (context.Depth > context.Settings.MaxDepth)
            return null;

        foreach (var propertyInfo in type.GetProperties())
        {
            context.Levels.Add(propertyInfo.Name);
            propertyInfo.SetValue(element, GetValueForType(propertyInfo.PropertyType, context, propertyInfo.Name));
            context.Levels.RemoveAt(context.Levels.Count - 1);
        }

        return element;
    }

    private static object GetValueForType(Type type, AutoFillContext context, string propertyName)
    {
        if (context.IgnoredMembers.Contains(string.Join(".", context.Levels)))
            return null;
        if (type == typeof(int))
            return context.GetNextInt();
        if (type == typeof(char))
            return (char) context.GetNextInt();
        if (type == typeof(byte))
            return (byte) context.GetNextInt();
        if (type == typeof(short))
            return (short) context.GetNextInt();
        if (type == typeof(long))
            return (long) context.GetNextInt();
        if (type == typeof(float))
            return (float) context.GetNextInt();
        if (type == typeof(double))
            return (double) context.GetNextInt();
        if (type == typeof(decimal))
            return (decimal) context.GetNextInt();
        if (type == typeof(bool))
            return true;
        if (type == typeof(string))
        {
            var value = GenerateStringValue(propertyName);
            if (context.Flags.HasFlag(AutoFillFlags.RandomizeString))
                value.Append("-").Append(RngUtils.GetRandomHexString(8));
            return value.ToString();
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return GetValueForType(type.GetGenericArguments()[0], context, propertyName);

        foreach (var interfaceType in type.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var elementListType = interfaceType.GenericTypeArguments[0];
                var listType = typeof(List<>).MakeGenericType(elementListType);
                var list = (IList) Activator.CreateInstance(listType);
                for (int i = 0; i < 3; i++)
                    list.Add(GetValueForType(elementListType, context, propertyName + i));

                return list;
            }
        }

        return CreateElement(type, context);
    }

    private static StringBuilder GenerateStringValue(string propertyName)
    {
        var value = new StringBuilder("some");
        var current = new StringBuilder();
        foreach (var c in propertyName)
        {
            if (c >= 'A' && c <= 'Z' && current.Length > 0)
            {
                value.Append("-").Append(current.ToString().ToLowerInvariant());
                current.Clear();
            }

            current.Append(c);
        }

        if (current.Length > 0)
        {
            value.Append("-").Append(current.ToString().ToLowerInvariant());
            current.Clear();
        }

        return value;
    }
}