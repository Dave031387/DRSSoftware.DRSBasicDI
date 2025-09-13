namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;
using IContainer = Interfaces.IContainer;

/// <summary>
/// The <see cref="DependencyList" /> class is used for storing and retrieving
/// <see cref="IDependency" /> objects representing application dependencies.
/// </summary>
/// <remarks>
/// This class is instantiated only by the <see cref="ServiceLocator" /> class.
/// </remarks>
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
    /// <remarks>
    /// This constructor is intended for use in unit tests only.
    /// </remarks>
    internal DependencyList() : this("")
    {
    }

    /// <summary>
    /// Create a new instance of the <see cref="DependencyList" /> class.
    /// </summary>
    private DependencyList(string _)
    {
        Dependency containerDependency = new(typeof(IContainer),
                                             typeof(Container),
                                             DependencyLifetime.Singleton,
                                             EmptyKey);
        _dependencies[containerDependency.DependencyServiceKey] = containerDependency;
    }

    /// <summary>
    /// Gets the number of dependencies in the container.
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
        ServiceKey serviceKey = dependency.DependencyServiceKey;

        if (!_dependencies.ContainsKey(serviceKey))
        {
            lock (_lock)
            {
                if (!_dependencies.ContainsKey(serviceKey))
                {
                    _dependencies[serviceKey] = dependency;
                    return;
                }
            }
        }

        string msg = FormatMessage(MsgDuplicateDependency, dependency.DependencyType, dependency.ResolvingKey);
        throw new ContainerBuildException(msg);
    }

    /// <summary>
    /// Get the <see cref="IDependency" /> object associated with the given
    /// <typeparamref name="TDependency" /> type and <paramref name="resolvingKey" />.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of dependency to be retrieved.
    /// </typeparam>
    /// <param name="resolvingKey">
    /// An optional key that uniquely identifies the dependency to be retrieved.
    /// </param>
    /// <returns>
    /// The <see cref="IDependency" /> object corresponding to the specified
    /// <typeparamref name="TDependency" /> type and <paramref name="resolvingKey" />.
    /// </returns>
    public IDependency Get<TDependency>(string resolvingKey) where TDependency : class
    {
        ServiceKey serviceKey = new(typeof(TDependency), resolvingKey);

        lock (_lock)
        {
            if (_dependencies.TryGetValue(serviceKey, out IDependency? dependency))
            {
                if (dependency is null)
                {
                    string msg = FormatMessage(MsgNullDependencyObject, serviceKey.Type, serviceKey.ResolvingKey);
                    throw new DependencyInjectionException(msg);
                }

                return dependency;
            }
            else
            {
                string msg = FormatMessage(MsgDependencyMappingNotFound, serviceKey.Type, serviceKey.ResolvingKey);
                throw new DependencyInjectionException(msg);
            }
        }
    }
}