namespace DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="IDependencyResolver" /> interface defines the methods implemented by the
/// dependency resolver object.
/// </summary>
internal interface IDependencyResolver : IDisposable
{
    /// <summary>
    /// Retrieve the resolving object for the given dependency type
    /// <typeparamref name="TDependency" />.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type that is to be resolved.
    /// </typeparam>
    /// <param name="key">
    /// An optional key used to identify the specific resolving object to be retrieved.
    /// </param>
    /// <returns>
    /// An instance of the resolving class type.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    public TDependency Resolve<TDependency>(string key) where TDependency : class;

    /// <summary>
    /// Retrieve the resolving object for the given dependency type
    /// <typeparamref name="TDependency" />. The object will be constructed using the given
    /// constructor <paramref name="parameters" />.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type that is to be resolved.
    /// </typeparam>
    /// <param name="parameters">
    /// The list of constructor parameters used for constructing the resolving object.
    /// </param>
    /// <param name="key">
    /// An optional key used to identify the specific resolving object to be retrieved.
    /// </param>
    /// <returns>
    /// An instance of the resolving class type.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    public TDependency Resolve<TDependency>(object[] parameters, string key) where TDependency : class;

    /// <summary>
    /// Set the scoped <see cref="IResolvingObjectsService" /> to the specified
    /// <paramref name="resolvingObjectsService" /> instance.
    /// </summary>
    /// <param name="resolvingObjectsService">
    /// The scoped <see cref="IResolvingObjectsService" /> instance to be set.
    /// </param>
    /// <exception cref="DependencyInjectionException" />
    public void SetScopedService(IResolvingObjectsService resolvingObjectsService);
}