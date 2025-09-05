namespace DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="IContainerBase" /> interface defines methods for resolving dependencies. It is
/// the base interface for the scoped and non-scoped dependency injection containers.
/// </summary>
/// <remarks>
/// This interface inherits from the <see cref="IDisposable" /> interface.
/// </remarks>
public interface IContainerBase : IDisposable
{
    /// <summary>
    /// Gets an instance of the resolving type that is mapped to the given dependency type
    /// <typeparamref name="TDependency" /> and <paramref name="resolvingKey" />. Constructor
    /// dependencies will be resolved automatically.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type that is to be retrieved.
    /// </typeparam>
    /// <param name="resolvingKey">
    /// An optional key used to identify the specific resolving object to be retrieved.
    /// </param>
    /// <returns>
    /// The resolving object for the given dependency type <typeparamref name="TDependency" /> and
    /// <paramref name="resolvingKey" />, or <see langword="null" /> if the resolving object can't
    /// be determined.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    public TDependency Resolve<TDependency>(string resolvingKey = EmptyKey) where TDependency : class;

    /// <summary>
    /// Gets an instance of the resolving type that is mapped to the given dependency type
    /// <typeparamref name="TDependency" /> and <paramref name="resolvingKey" />. The instance will
    /// be created using the specified constructor <paramref name="parameters" />.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type that is to be retrieved.
    /// </typeparam>
    /// <param name="parameters">
    /// An array containing all parameters required by the constructor of the resolving type.
    /// </param>
    /// <param name="resolvingKey">
    /// An optional key used to identify the specific resolving object to be retrieved.
    /// </param>
    /// <returns>
    /// The resolving object for the given dependency type <typeparamref name="TDependency" /> and
    /// <paramref name="resolvingKey" />, or <see langword="null" /> if the resolving object can't
    /// be determined.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    public TDependency Resolve<TDependency>(object[] parameters, string resolvingKey = EmptyKey) where TDependency : class;
}