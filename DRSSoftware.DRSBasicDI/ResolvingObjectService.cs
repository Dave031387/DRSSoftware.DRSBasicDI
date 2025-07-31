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
    /// <see cref="_resolvingObjects" /> field.
    /// </summary>
    private readonly object _lock = new();

    /// <summary>
    /// Create an instance of the <see cref="ResolvingObjectsService" /> class.
    /// </summary>
    internal ResolvingObjectsService() : this(ServiceLocater.Instance)
    {
    }

    /// <summary>
    /// Constructor for the <see cref="ResolvingObjectsService" /> class. Intended for unit testing
    /// only.
    /// </summary>
    /// <param name="serviceLocater">
    /// A service locater object that should provide mock instances of the requested dependencies.
    /// </param>
    internal ResolvingObjectsService(IServiceLocater serviceLocater)
        => DependencyList = serviceLocater.Get<IDependencyListConsumer>();

    /// <summary>
    /// Get a reference to the <see cref="IDependencyListBuilder" /> object.
    /// </summary>
    private IDependencyListConsumer DependencyList
    {
        get;
    }

    /// <summary>
    /// Add the given <paramref name="resolvingObject" /> to the list of resolving objects if no
    /// object currently exists for the given dependency type <typeparamref name="TDependency" />
    /// having the specified <paramref name="key" />.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type being resolved.
    /// </typeparam>
    /// <param name="resolvingObject">
    /// The resolving object to be added for the given dependency type
    /// <typeparamref name="TDependency" />.
    /// </param>
    /// <param name="key">
    /// An optional key used to identify the specific <paramref name="resolvingObject" /> to be
    /// added.
    /// </param>
    /// <returns>
    /// The <paramref name="resolvingObject" /> or the object retrieved from the list of resolving
    /// objects if one already exists for the given dependency type
    /// <typeparamref name="TDependency" /> and <paramref name="key" />.
    /// </returns>
    public TDependency Add<TDependency>(TDependency resolvingObject, string key) where TDependency : class
    {
        IDependency dependency = DependencyList.Get<TDependency>(key);
        ServiceKey serviceKey = ServiceKey.GetServiceResolvingKey(dependency);

        lock (_lock)
        {
            if (TryGetResolvingObject(out TDependency? value, serviceKey))
            {
                // If we get here then a resolving object for the given dependency type has already
                // been added to the container and the resolving object that was added isn't null.
                return value!;
            }

            // If we get here then a resolving object hasn't yet been added to the container for the
            // given dependency type.
            _resolvingObjects[serviceKey] = resolvingObject;
            return resolvingObject;
        }
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
    /// <param name="key">
    /// An optional key used to identify the specific <paramref name="resolvingObject" /> to be
    /// retrieved.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the given dependency type has been resolved, otherwise
    /// <see langword="false" />.
    /// </returns>
    public bool TryGetResolvingObject<TDependency>(out TDependency? resolvingObject, string key) where TDependency : class
    {
        IDependency dependency = DependencyList.Get<TDependency>(key);
        ServiceKey serviceKey = ServiceKey.GetServiceResolvingKey(dependency);

        lock (_lock)
        {
            return TryGetResolvingObject(out resolvingObject, serviceKey);
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
    /// <param name="serviceKey">
    /// The service key used to identify the specific <paramref name="resolvingObject" /> to be
    /// returned.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the given dependency type has been resolved, otherwise
    /// <see langword="false" />.
    /// </returns>
    private bool TryGetResolvingObject<TDependency>(out TDependency? resolvingObject, ServiceKey serviceKey) where TDependency : class
    {
        if (_resolvingObjects.TryGetValue(serviceKey, out object? value))
        {
            if (value is not null)
            {
                resolvingObject = (TDependency)value;
                return true;
            }
        }

        resolvingObject = default;
        return false;
    }
}