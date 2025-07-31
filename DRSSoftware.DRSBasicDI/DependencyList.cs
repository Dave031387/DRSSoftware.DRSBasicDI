namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;
using IContainer = Interfaces.IContainer;

/// <summary>
/// The <see cref="DependencyList" /> class is used to storing and retrieving
/// <see cref="IDependency" /> objects.
/// </summary>
internal sealed class DependencyList : IDependencyListBuilder, IDependencyListConsumer
{
    /// <summary>
    /// A list of <see cref="IDependency" /> objects that have been added to the dependency
    /// injection container.
    /// </summary>
    internal readonly Dictionary<ServiceKey, IDependency> _dependencies = [];

    /// <summary>
    /// A lock object used to ensure thread safety when accessing or saving
    /// <see cref="IDependency" /> objects.
    /// </summary>
    private readonly object _lock = new();

    /// <summary>
    /// Create a new instance of the <see cref="DependencyList" /> class.
    /// </summary>
    internal DependencyList()
    {
        IDependency containerDependency = new Dependency(typeof(IContainer),
                                                         typeof(Container),
                                                         DependencyLifetime.Singleton,
                                                         null,
                                                         EmptyKey);
        ServiceKey serviceKey = ServiceKey.GetServiceDependencyKey(containerDependency);
        _dependencies[serviceKey] = containerDependency;
    }

    /// <summary>
    /// Get the number of dependencies in the container.
    /// </summary>
    public int Count => _dependencies.Count;

    /// <summary>
    /// Add the given <paramref name="dependency" /> to the list of dependencies.
    /// </summary>
    /// <param name="dependency">
    /// The <see cref="IDependency" /> object to be added to the list of dependencies.
    /// </param>
    /// <exception cref="ContainerBuildException" />
    public void Add(IDependency dependency)
    {
        ServiceKey serviceKey = ServiceKey.GetServiceDependencyKey(dependency);

        lock (_lock)
        {
            if (_dependencies.ContainsKey(serviceKey))
            {
                string msg = FormatMessage(MsgDuplicateDependency, dependency.DependencyType, dependency.Key);
                throw new ContainerBuildException(msg);
            }

            _dependencies[serviceKey] = dependency;
        }
    }

    /// <summary>
    /// Get the <see cref="IDependency" /> object for the given dependency type
    /// <typeparamref name="TDependency" /> and <paramref name="key" /> value.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of dependency to be retrieved.
    /// </typeparam>
    /// <param name="key">
    /// An optional key used to identify the specific <see cref="IDependency" /> object to be
    /// retrieved.
    /// </param>
    /// <returns>
    /// The <see cref="IDependency" /> instance corresponding to the given dependency type
    /// <typeparamref name="TDependency" /> and <paramref name="key" /> value.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    public IDependency Get<TDependency>(string key) where TDependency : class
        => Get(typeof(TDependency), key);

    /// <summary>
    /// Get the <see cref="IDependency" /> object for the given <paramref name="dependencyType" />
    /// and <paramref name="key" /> value.
    /// </summary>
    /// <param name="dependencyType">
    /// The type of dependency to be retrieved.
    /// </param>
    /// <param name="key">
    /// An optional key used to identify the specific <see cref="IDependency" /> object to be
    /// retrieved.
    /// </param>
    /// <returns>
    /// The <see cref="IDependency" /> instance corresponding to the given
    /// <paramref name="dependencyType" /> and <paramref name="key" /> value.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    public IDependency Get(Type dependencyType, string key)
    {
        ServiceKey serviceKey = ServiceKey.GetServiceKey(dependencyType, key);

        lock (_lock)
        {
            if (_dependencies.TryGetValue(serviceKey, out IDependency? dependency))
            {
                if (dependency is null)
                {
                    string msg = FormatMessage(MsgNullDependencyObject, dependencyType, key);
                    throw new DependencyInjectionException(msg);
                }

                return dependency;
            }
            else
            {
                string msg = FormatMessage(MsgDependencyMappingNotFound, dependencyType, key);
                throw new DependencyInjectionException(msg);
            }
        }
    }
}