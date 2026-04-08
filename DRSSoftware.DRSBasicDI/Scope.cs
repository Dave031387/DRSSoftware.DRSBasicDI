namespace DRSSoftware.DRSBasicDI;

/// <summary>
/// The <see cref="Scope" /> class implements a basic dependency injection container for managing
/// and resolving scoped dependencies. Each <see cref="Scope" /> object manages scoped dependencies
/// within a single dependency scope.
/// </summary>
/// <remarks>
/// This class is instantiated only by the <see cref="ServiceLocator" /> class.
/// </remarks>
internal sealed class Scope : ContainerBase, IScope
{
    /// <summary>
    /// Constructor for the <see cref="Scope" /> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// A service locator object that should provide mock instances of the requested dependencies.
    /// </param>
    /// <remarks>
    /// This constructor is intended for unit testing purposes only.
    /// </remarks>
    internal Scope(IServiceLocator serviceLocator)
    {
        DependencyResolver = serviceLocator.Get<IDependencyResolver>(Scoped);
        IResolvingObjectsService resolvingObjectsService = serviceLocator.Get<IResolvingObjectsService>(Scoped);
        DependencyResolver.SetScopedService(resolvingObjectsService);
    }

    /// <summary>
    /// Create a new instance of the <see cref="Scope" /> class.
    /// </summary>
    /// <param name="containerKey">
    /// A <see langword="string" /> used to identify the specific <see cref="ServiceLocator" />
    /// instance to use in resolving dependencies.
    /// </param>
    /// <remarks>
    /// This constructor is declared <see langword="private" /> and is only ever called by the
    /// <see cref="ServiceLocator" /> class when creating a new <see cref="Scope" /> instance.
    /// </remarks>
    private Scope(string containerKey) : this(ServiceLocator.GetInstance(containerKey))
    {
    }
}