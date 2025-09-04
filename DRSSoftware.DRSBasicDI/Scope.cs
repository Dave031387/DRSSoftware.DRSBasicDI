namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="Scope" /> class implements a basic dependency injection container for managing
/// and resolving scoped dependencies.
/// </summary>
/// <remarks>
/// Each <see cref="Scope" /> object manages scoped dependencies within a single dependency scope.
/// </remarks>
internal sealed class Scope : ContainerBase, IScope
{
    /// <summary>
    /// Create a new instance of the <see cref="Scope" /> class.
    /// </summary>
    /// <param name="containerKey">
    /// A <see langword="string"/> used to identify the specific <see cref="ServiceLocator"/>
    /// instance to use in resolving dependencies.
    /// </param>
    internal Scope(string containerKey) : this(ServiceLocator.GetInstance(containerKey))
    {
    }

    /// <summary>
    /// Constructor for the <see cref="Scope" /> class. Intended for unit testing only.
    /// </summary>
    /// <param name="serviceLocator">
    /// A service locator object that should provide mock instances of the requested dependencies.
    /// </param>
    internal Scope(IServiceLocator serviceLocator)
    {
        DependencyResolver = serviceLocator.Get<IDependencyResolver>(Scoped);
        IResolvingObjectsService resolvingObjectsService = serviceLocator.Get<IResolvingObjectsService>(Scoped);
        DependencyResolver.SetScopedService(resolvingObjectsService);
    }
}