namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;
using System.Reflection;

/// <summary>
/// The <see cref="ObjectConstructor" /> class is used to construct resolving objects for a given
/// dependency type.
/// </summary>
/// <remarks>
/// This class is instantiated only by the <see cref="ServiceLocator" /> class.
/// </remarks>
internal sealed class ObjectConstructor : IObjectConstructor
{
    /// <summary>
    /// Constructs a new instance of the <see cref="ObjectConstructor" /> class.
    /// </summary>
    /// <remarks>
    /// This constructor is intended for use in unit tests only.
    /// </remarks>
    internal ObjectConstructor() : this(EmptyKey)
    {
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="ObjectConstructor" /> class.
    /// </summary>
    private ObjectConstructor(string _)
    {
    }

    /// <summary>
    /// Constructs an object of type <typeparamref name="TDependency" /> using the specified
    /// <paramref name="constructorInfo" /> and <paramref name="parameterValues" />.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type to be constructed.
    /// </typeparam>
    /// <param name="constructorInfo">
    /// The constructor information for the resolving type that was mapped to the dependency type
    /// <typeparamref name="TDependency" />.
    /// </param>
    /// <param name="parameterValues">
    /// The constructor parameter values to be used for constructing the resolving type.
    /// </param>
    /// <param name="resolvingKey">
    /// An optional key used to identify the specific resolving object to be constructed.
    /// </param>
    /// <returns>
    /// The resolving object cast to the dependency type <typeparamref name="TDependency" />.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    public TDependency Construct<TDependency>(ConstructorInfo constructorInfo, object[] parameterValues, string resolvingKey) where TDependency : class
    {
        try
        {
            if (constructorInfo.Invoke(parameterValues) is not TDependency resolvingObject)
            {
                string msg = FormatMessage<TDependency>(MsgResolvingObjectNotCreated, resolvingKey, constructorInfo.DeclaringType);
                throw new DependencyInjectionException(msg);
            }

            return resolvingObject;
        }
        catch (Exception ex)
        {
            Type declaringType = constructorInfo is null || constructorInfo.DeclaringType is null
                ? typeof(TDependency)
                : constructorInfo.DeclaringType;
            string msg = FormatMessage<TDependency>(MsgErrorDuringConstruction, resolvingKey, declaringType);
            throw new DependencyInjectionException(msg, ex);
        }
    }
}