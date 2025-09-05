namespace DRSSoftware.DRSBasicDI.Interfaces;

using System.Reflection;

/// <summary>
/// The <see cref="IObjectConstructor" /> defines a method for constructing the resolving object for
/// a given dependency type.
/// </summary>
internal interface IObjectConstructor
{
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
    public TDependency Construct<TDependency>(ConstructorInfo constructorInfo, object[] parameterValues, string resolvingKey) where TDependency : class;
}