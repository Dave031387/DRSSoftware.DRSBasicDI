namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="Scope" /> class manages the creation and disposing of scoped dependencies.
/// </summary>
internal sealed class Scope : ContainerBase, IScope
{
    /// <summary>
    /// Create a new instance of the <see cref="Scope" /> class.
    /// </summary>
    internal Scope() : this(ServiceLocater.Instance)
    {
    }

    /// <summary>
    /// Constructor for the <see cref="Scope" /> class. Intended for unit testing only.
    /// </summary>
    /// <param name="serviceLocater">
    /// A service locater object that should provide mock instances of the requested dependencies.
    /// </param>
    internal Scope(IServiceLocater serviceLocater)
    {
        DependencyResolver = serviceLocater.Get<IDependencyResolver>(Scoped);
        IResolvingObjectsService resolvingObjectsService = serviceLocater.Get<IResolvingObjectsService>(Scoped);
        DependencyResolver.SetScopedService(resolvingObjectsService);
    }
}