namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="Container" /> class implements a basic dependency injection container for
/// managing and resolving non-scoped dependencies.
/// </summary>
internal sealed class Container : ContainerBase, IContainer
{
    /// <summary>
    /// Create a new instance of the <see cref="Container" /> class.
    /// </summary>
    internal Container() : this(DRSBasicDI.ServiceLocater.Instance)
    {
    }

    /// <summary>
    /// Constructor for the <see cref="Container" /> class. Intended for unit testing only.
    /// </summary>
    /// <param name="serviceLocater">
    /// A service locater object that should provide mock instances of the requested dependencies.
    /// </param>
    internal Container(IServiceLocater serviceLocater)
    {
        ServiceLocater = serviceLocater;
        IResolvingObjectsService resolvingObjectsService = ServiceLocater.Get<IResolvingObjectsService>(NonScoped);
        _ = resolvingObjectsService.Add<IContainer>(this, EmptyKey);
        DependencyResolver = ServiceLocater.Get<IDependencyResolver>(NonScoped);
    }

    /// <summary>
    /// Get a reference to the <see cref="IServiceLocater" /> object.
    /// </summary>
    private IServiceLocater ServiceLocater
    {
        get;
    }

    /// <summary>
    /// Create a new <see cref="IScope" /> object to be used in managing a dependency scope.
    /// </summary>
    /// <returns>
    /// A new <see cref="IScope" /> object.
    /// </returns>
    public IScope CreateScope() => ServiceLocater.Get<IScope>();
}