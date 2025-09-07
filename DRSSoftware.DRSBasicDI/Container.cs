namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="Container" /> class implements a basic dependency injection container for
/// managing and resolving non-scoped dependencies.
/// </summary>
internal sealed class Container : ContainerBase, IContainer
{
    /// <summary>
    /// A dictionary of <see cref="IContainer" /> objects.
    /// </summary>
    private static readonly Dictionary<string, IContainer> _containers = [];

    /// <summary>
    /// A lock object used to ensure thread safety.
    /// </summary>
    private static readonly object _lock = new();

    /// <summary>
    /// Create a new instance of the <see cref="Container" /> class.
    /// </summary>
    /// <param name="containerKey">
    /// A unique <see langword="string" /> value used to identify and retrieve a specific instance
    /// of the <see cref="Container" /> object.
    /// </param>
    internal Container(string containerKey) : this(DRSBasicDI.ServiceLocator.GetInstance(containerKey), containerKey)
    {
    }

    /// <summary>
    /// Constructor for the <see cref="Container" /> class. Intended for unit testing only.
    /// </summary>
    /// <param name="serviceLocator">
    /// A service locator object that should provide mock instances of the requested dependencies.
    /// </param>
    /// <param name="containerKey">
    /// A unique <see langword="string" /> value used to identify and retrieve a specific instance
    /// of the <see cref="Container" /> object.
    /// </param>
    internal Container(IServiceLocator serviceLocator, string containerKey)
    {
        ContainerKey = containerKey;
        RegisterContainer(containerKey);
        ServiceLocator = serviceLocator;
        IResolvingObjectsService resolvingObjectsService = ServiceLocator.Get<IResolvingObjectsService>(NonScoped);
        _ = resolvingObjectsService.Add<IContainer>(this, EmptyKey);
        DependencyResolver = ServiceLocator.Get<IDependencyResolver>(NonScoped);
    }

    /// <summary>
    /// Gets a <see langword="string" /> value used to identify this specific
    /// <see cref="Container" /> instance.
    /// </summary>
    public string ContainerKey
    {
        get;
        private init;
    }

    /// <summary>
    /// Get a reference to the <see cref="IServiceLocator" /> object.
    /// </summary>
    private IServiceLocator ServiceLocator
    {
        get;
    }

    /// <summary>
    /// Retrieves an instance of the container associated with the specified
    /// <paramref name="containerKey" />.
    /// </summary>
    /// <param name="containerKey">
    /// The <see langword="string" /> value used to identify the container. If not specified, an
    /// empty string is used.
    /// </param>
    /// <returns>
    /// The container instance associated with the specified key.
    /// </returns>
    /// <exception cref="DependencyInjectionException">
    /// Thrown if no container has been built for the specified <paramref name="containerKey" /> or
    /// if the key is <see langword="null" />.
    /// </exception>
    public static IContainer GetInstance(string containerKey = EmptyKey)
    {
        if (containerKey is null)
        {
            throw new DependencyInjectionException(MsgNullContainerKey);
        }

        if (_containers.TryGetValue(containerKey, out IContainer? value))
        {
            return value;
        }

        throw new DependencyInjectionException(MsgContainerNotBuilt);
    }

    /// <summary>
    /// Create a new <see cref="IScope" /> object to be used in managing a dependency scope.
    /// </summary>
    /// <returns>
    /// A new <see cref="IScope" /> object.
    /// </returns>
    public IScope CreateScope() => ServiceLocator.Get<IScope>();

    /// <summary>
    /// Registers the current container instance with the specified key.
    /// </summary>
    /// <remarks>
    /// This method ensures thread safety when registering the container. If the key is already
    /// registered, an exception is thrown to prevent duplicate registrations.
    /// </remarks>
    /// <param name="containerKey">
    /// The unique key used to identify the container. Must not be null.
    /// </param>
    /// <exception cref="DependencyInjectionException">
    /// Thrown if a container is already registered with the specified key.
    /// </exception>
    private void RegisterContainer(string containerKey)
    {
        if (!_containers.ContainsKey(containerKey))
        {
            lock (_lock)
            {
                if (!_containers.ContainsKey(containerKey))
                {
                    _containers[containerKey] = this;
                    return;
                }
            }
        }

        throw new DependencyInjectionException(MsgContainerAlreadyBuilt);
    }
}