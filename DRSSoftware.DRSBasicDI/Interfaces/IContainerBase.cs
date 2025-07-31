namespace DRSSoftware.DRSBasicDI.Interfaces;

public interface IContainerBase : IDisposable
{
    /// <summary>
    /// Gets an instance of the resolving type that is mapped to the given dependency type
    /// <typeparamref name="TDependency" /> and resolving <paramref name="key" />. Constructor
    /// dependencies will be resolved automatically.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type that is to be retrieved.
    /// </typeparam>
    /// <param name="key">
    /// An optional key used to identify the specific resolving object to be retrieved.
    /// </param>
    /// <returns>
    /// The resolving object for the given dependency type <typeparamref name="TDependency" /> and
    /// resolving <paramref name="key" />, or <see langword="null" /> if the resolving object can't
    /// be determined.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    public TDependency Resolve<TDependency>(string key = EmptyKey) where TDependency : class;

    /// <summary>
    /// Gets an instance of the resolving type that is mapped to the given dependency type
    /// <typeparamref name="TDependency" /> and resolving <paramref name="key" />. The instance will
    /// be created using the specified constructor <paramref name="parameters" />.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type that is to be retrieved.
    /// </typeparam>
    /// <param name="parameters">
    /// The complete list of parameters required by the constructor of the resolving type.
    /// </param>
    /// <param name="key">
    /// An optional key used to identify the specific resolving object to be retrieved.
    /// </param>
    /// <returns>
    /// The resolving object for the given dependency type <typeparamref name="TDependency" /> and
    /// resolving <paramref name="key" />, or <see langword="null" /> if the resolving object can't
    /// be determined.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    public TDependency Resolve<TDependency>(object[] parameters, string key = EmptyKey) where TDependency : class;
}
