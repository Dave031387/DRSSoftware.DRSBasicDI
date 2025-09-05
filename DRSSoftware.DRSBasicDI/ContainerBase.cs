namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="ContainerBase" /> class is an abstract base class that is implemented in both the
/// <see cref="Container" /> and <see cref="Scope" /> classes. The class implements methods for
/// resolving and disposing of dependencies.
/// </summary>
internal abstract class ContainerBase : IContainerBase
{
    /// <summary>
    /// Get a reference to the <see cref="IDependencyResolver" /> object.
    /// </summary>
    protected IDependencyResolver DependencyResolver
    {
        get;
        init;
    } = null!;

    /// <summary>
    /// Dispose of any resources owned by any dependency objects that have been built.
    /// </summary>
    public void Dispose() => DependencyResolver.Dispose();

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
    public TDependency Resolve<TDependency>(string resolvingKey = EmptyKey) where TDependency : class
        => DependencyResolver.Resolve<TDependency>(resolvingKey);

    /// <summary>
    /// Gets an instance of the resolving type that is mapped to the given dependency type
    /// <typeparamref name="TDependency" /> and <paramref name="resolvingKey" />. The instance will
    /// be created using the specified constructor <paramref name="parameters" />.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type that is to be retrieved.
    /// </typeparam>
    /// <param name="parameters">
    /// An array containing all the parameters required by the constructor of the resolving type.
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
    public TDependency Resolve<TDependency>(object[] parameters, string resolvingKey = EmptyKey) where TDependency : class
        => DependencyResolver.Resolve<TDependency>(parameters, resolvingKey);
}