namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="ResolvingObjectsService" /> class manages a dictionary of resolving objects for
/// the singleton and scoped dependencies contained in the dependency injection container.
/// </summary>
internal sealed class ResolvingObjectsService : IResolvingObjectsService
{
    /// <summary>
    /// A dictionary of resolving objects.
    /// </summary>
    internal readonly Dictionary<ServiceKey, object> _resolvingObjects = [];

    /// <summary>
    /// A lock object used to ensure thread safety when accessing/modifying the
    /// <see cref="_resolvingObjects" /> dictionary.
    /// </summary>
    private readonly object _lock = new();

    /// <summary>
    /// Create an instance of the <see cref="ResolvingObjectsService" /> class.
    /// </summary>
    /// <param name="containerKey">
    /// A <see langword="string" /> used to identify the specific <see cref="ServiceLocator" />
    /// instance to use in resolving dependencies.
    /// </param>
    private ResolvingObjectsService(string containerKey) : this(ServiceLocator.GetInstance(containerKey))
    {
    }

    /// <summary>
    /// Constructor for the <see cref="ResolvingObjectsService" /> class. Intended for unit testing
    /// only.
    /// </summary>
    /// <param name="serviceLocator">
    /// A service locator object that should provide mock instances of the requested dependencies.
    /// </param>
    internal ResolvingObjectsService(IServiceLocator serviceLocator)
        => DependencyList = serviceLocator.Get<IDependencyListConsumer>();

    /// <summary>
    /// Get a reference to the <see cref="IDependencyListConsumer" /> object.
    /// </summary>
    private IDependencyListConsumer DependencyList
    {
        get;
        init;
    }

    /// <summary>
    /// Add the given <paramref name="resolvingObject" /> to the list of resolving objects if no
    /// object currently exists for the given dependency type <typeparamref name="TDependency" />
    /// having the specified <paramref name="resolvingKey" />.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type being resolved.
    /// </typeparam>
    /// <param name="resolvingObject">
    /// The resolving object to be added for the given dependency type
    /// <typeparamref name="TDependency" />.
    /// </param>
    /// <param name="resolvingKey">
    /// An optional key used to identify the specific <paramref name="resolvingObject" /> to be
    /// added.
    /// </param>
    /// <returns>
    /// The <paramref name="resolvingObject" /> or the object retrieved from the list of resolving
    /// objects if one already exists for the given dependency type
    /// <typeparamref name="TDependency" /> and <paramref name="resolvingKey" />.
    /// </returns>
    public TDependency Add<TDependency>(TDependency resolvingObject, string resolvingKey) where TDependency : class
    {
        IDependency dependency = DependencyList.Get<TDependency>(resolvingKey);
        ServiceKey serviceKey = dependency.ResolvingServiceKey;

        if (!_resolvingObjects.ContainsKey(serviceKey))
        {
            lock (_lock)
            {
                if (!_resolvingObjects.ContainsKey(serviceKey))
                {
                    _resolvingObjects[serviceKey] = resolvingObject;
                }
            }
        }

        return (TDependency)_resolvingObjects[serviceKey];
    }

    /// <summary>
    /// Remove all objects from the list of resolved dependencies. Call Dispose on each object that
    /// implements the <see cref="IDisposable" /> interface.
    /// </summary>
    public void Dispose()
    {
        lock (_lock)
        {
            foreach (ServiceKey serviceKey in _resolvingObjects.Keys)
            {
                if (_resolvingObjects[serviceKey] is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                _ = _resolvingObjects.Remove(serviceKey);
            }
        }
    }

    /// <summary>
    /// Check to see if the specified dependency type has been resolved and, if it has, return the
    /// resolving object.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type whose resolving object is to be retrieved.
    /// </typeparam>
    /// <param name="resolvingObject">
    /// The resolved dependency object, or <see langword="null" /> if the dependency type hasn't yet
    /// been resolved.
    /// </param>
    /// <param name="dependency">
    /// The <see cref="IDependency" /> object that is being resolved.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the given dependency type has been resolved, otherwise
    /// <see langword="false" />.
    /// </returns>
    public bool TryGetResolvingObject<TDependency>(out TDependency? resolvingObject, IDependency dependency) where TDependency : class
    {
        ServiceKey resolvingServiceKey = dependency.ResolvingServiceKey;

        if (_resolvingObjects.TryGetValue(resolvingServiceKey, out object? value))
        {
            resolvingObject = value as TDependency;
            return true;
        }

        resolvingObject = null;
        return false;
    }
}