namespace DRSSoftware.DRSBasicDI.Extensions;

using DRSSoftware.DRSBasicDI.Attributes;
using System.Reflection;

/// <summary>
/// The <see cref="TypeExtensions" /> class extends the <see cref="Type" /> class by adding a couple
/// methods that are used by the <see cref="DRSBasicDI" /> class library.
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    /// The <see cref="BindingFlags" /> used to find constructors for the implementation types.
    /// </summary>
    private const BindingFlags ConstructorBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// A dictionary of predefined types whose key is the predefined type and whose value is the
    /// friendly name for the predefined type.
    /// </summary>
    private static readonly Dictionary<Type, string> _typeTranslationDictionary = new()
    {
        {typeof(bool), BoolFriendlyName},
        {typeof(byte), ByteFriendlyName},
        {typeof(char), CharFriendlyName},
        {typeof(decimal), DecimalFriendlyName},
        {typeof(double), DoubleFriendlyName},
        {typeof(float), FloatFriendlyName},
        {typeof(int), IntFriendlyName},
        {typeof(long), LongFriendlyName},
        {typeof(object), ObjectFriendlyName},
        {typeof(sbyte), SByteFriendlyName},
        {typeof(short), ShortFriendlyName},
        {typeof(string), StringFriendlyName},
        {typeof(uint), UIntFriendlyName},
        {typeof(ulong), ULongFriendlyName},
        {typeof(ushort), UShortFriendlyName},
        {typeof(void), VoidFriendlyName}
    };

    /// <summary>
    /// An extension method for the <see cref="Type" /> class that returns the dependency injection
    /// constructor info for the given <see cref="Type" />.
    /// </summary>
    /// <remarks>
    /// If one of the constructors has been attributed with the
    /// <see cref="DIConstructorAttribute" /> then the <see cref="ConstructorInfo" /> for that
    /// constructor will be returned. <br /> Otherwise, if there is more than one constructor for
    /// the given class type, then the info for the constructor having the most parameters will be
    /// returned.
    /// </remarks>
    /// <param name="type">
    /// The class type for which we want to retrieve the constructor info.
    /// </param>
    /// <returns>
    /// The <see cref="ConstructorInfo" /> object for the given class type.
    /// </returns>
    /// <exception cref="InvalidOperationException" />
    internal static ConstructorInfo GetDIConstructorInfo(this Type type)
    {
        ConstructorInfo[] constructors = type.GetConstructors(ConstructorBindingFlags);

        if (constructors.Length < 1)
        {
            string msg = FormatMessage(MsgNoSuitableConstructors, resolvingType: type);
            throw new InvalidOperationException(msg);
        }

        int maxParameterCount = -1;
        int constructorIndex = -1;

        for (int i = 0; i < constructors.Length; i++)
        {
            DIConstructorAttribute? attribute = constructors[i].GetCustomAttribute<DIConstructorAttribute>();

            if (attribute is not null)
            {
                constructorIndex = i;
                break;
            }

            int parameterCount = constructors[i].GetParameters().Length;

            if (parameterCount > maxParameterCount)
            {
                maxParameterCount = parameterCount;
                constructorIndex = i;
            }
        }

        return constructors[constructorIndex];
    }

    /// <summary>
    /// An extension method for the <see cref="Type" /> class that returns the dependency injection
    /// constructor info having the specified number of parameters for the given
    /// <see cref="Type" />.
    /// </summary>
    /// <remarks>
    /// If one of the matching constructors has been attributed with the
    /// <see cref="DIConstructorAttribute" /> then the <see cref="ConstructorInfo" /> for that
    /// constructor will be returned. <br /> Otherwise, if there is more than one constructor for
    /// the given class type having the specified number of parameters, then the info for the last
    /// constructor found will be returned.
    /// </remarks>
    /// <param name="type">
    /// The class type for which we want to retrieve the constructor info.
    /// </param>
    /// <param name="parameterCount">
    /// Specifies the expected number of parameters to look for in the constructor.
    /// </param>
    /// <returns>
    /// The <see cref="ConstructorInfo" /> object having the specified number of parameters for the
    /// given class type.
    /// </returns>
    /// <exception cref="InvalidOperationException" />
    internal static ConstructorInfo GetDIConstructorInfo(this Type type, int parameterCount)
    {
        ConstructorInfo[] constructors = type.GetConstructors(ConstructorBindingFlags);

        if (constructors.Length < 1)
        {
            string msg = FormatMessage(MsgNoSuitableConstructors, resolvingType: type);
            throw new InvalidOperationException(msg);
        }

        int constructorIndex = -1;

        for (int i = 0; i < constructors.Length; i++)
        {
            if (constructors[i].GetParameters().Length != parameterCount)
            {
                continue;
            }

            constructorIndex = i;
            DIConstructorAttribute? attribute = constructors[i].GetCustomAttribute<DIConstructorAttribute>();

            if (attribute is not null)
            {
                break;
            }
        }

        if (constructorIndex > -1)
        {
            return constructors[constructorIndex];
        }

        string msg2 = FormatMessage(MsgConstructorNotFound, resolvingType: type);
        throw new InvalidOperationException(msg2);
    }

    /// <summary>
    /// An extension method for the <see cref="Type" /> class that returns a user-friendly name for
    /// the <see cref="Type" /> instance.
    /// </summary>
    /// <param name="type">
    /// The current instance of the <see cref="Type" /> class.
    /// </param>
    /// <returns>
    /// The friendly name for the current <see cref="Type" /> instance.
    /// </returns>
    /// <remarks>
    /// This method is recursive and is able to handle nested types.
    /// </remarks>
    internal static string GetFriendlyName(this Type type)
    {
        // Handle predefined types.
        if (_typeTranslationDictionary.TryGetValue(type, out string? predefinedTypeName))
        {
            return predefinedTypeName;
        }
        // Handle array types.
        else if (type.IsArray)
        {
            int rank = type.GetArrayRank();
            string commas = rank > 1
                ? new string(Comma, rank - 1)
                : string.Empty;
            Type arrayElementType = type.GetElementType()!;
            string arrayElementTypeName = arrayElementType.GetFriendlyName();
            return $"{arrayElementTypeName}[{commas}]";
        }
        // Handle nullable value types.
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            Type nullableType = type.GetGenericArguments()[0];
            string nullableTypeName = nullableType.GetFriendlyName();
            return $"{nullableTypeName}?";
        }
        // Handle generic types.
        else if (type.IsGenericType)
        {
            string genericTypeName = type.Name.Split(GenericSeparator)[0];
            Type[] genericParameterTypes = type.GetGenericArguments();
            string genericParameterTypeNames = string.Join(ListSeparator, genericParameterTypes.Select(GetFriendlyName));
            return $"{genericTypeName}<{genericParameterTypeNames}>";
        }
        // Everything else is a simple class type.
        else
        {
            return type.Name;
        }
    }
}