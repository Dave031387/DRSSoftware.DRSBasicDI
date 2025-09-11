namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;
using System.Reflection;
using IContainer = Interfaces.IContainer;

/// <summary>
/// The <see cref="ServiceLocator" /> class implements a basic service locator pattern for
/// dependency injection. This class is used strictly for resolving dependencies within the
/// <see cref="DRSBasicDI" /> class library.
/// </summary>
internal sealed class ServiceLocator : IServiceLocator
{
    /// <summary>
    /// The <see cref="BindingFlags" /> used to find constructors for the implementation types.
    /// </summary>
    private const BindingFlags ConstructorBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// A lock object used for ensuring thread safety.
    /// </summary>
    /// <remarks>
    /// This lock is used when updating the global dictionary of available service locator
    /// instances.
    /// </remarks>
    private static readonly object _globalLock = new();

    /// <summary>
    /// A collection of service locators, keyed by their associated string identifiers.
    /// </summary>
    /// <remarks>
    /// This dictionary is used to store and retrieve <see cref="IServiceLocator" /> instances based
    /// on their string keys. It is intended for internal use to manage service locators within the
    /// application.
    /// </remarks>
    private static readonly Dictionary<string, IServiceLocator> _serviceLocators = [];

    /// <summary>
    /// A lock object used for ensuring thread safety.
    /// </summary>
    /// <remarks>
    /// This lock is used when updating the local dictionary of services within a single service
    /// locator instance.
    /// </remarks>
    private readonly object _localLock = new();

    /// <summary>
    /// A dictionary of <see cref="Dependency" /> objects representing the various application
    /// dependencies in the <see cref="DRSBasicDI" /> class library.
    /// </summary>
    private readonly Dictionary<ServiceKey, Dependency> _services = [];

    /// <summary>
    /// A dictionary of singleton instances used to store all singleton dependency objects in the
    /// <see cref="DRSBasicDI" /> class library.
    /// </summary>
    private readonly Dictionary<ServiceKey, object> _singletonInstances = [];

    /// <summary>
    /// Create a new instance of the <see cref="ServiceLocator" /> class and register all of the
    /// dependencies for the <see cref="DRSBasicDI" /> class library.
    /// </summary>
    /// <remarks>
    /// This is a private constructor to prevent external instantiation of the
    /// <see cref="ServiceLocator" /> class. The static <see cref="GetInstance(string)" /> method
    /// must be used to obtain an instance of the <see cref="ServiceLocator" /> class.
    /// </remarks>
    private ServiceLocator(string containerKey)
    {
        ContainerKey = containerKey;
        Register<IContainer, Container>(DependencyLifetime.Singleton);
        Register<IDependencyListBuilder, DependencyList>(DependencyLifetime.Singleton);
        Register<IDependencyListConsumer, DependencyList>(DependencyLifetime.Singleton);
        Register<IDependencyResolver, DependencyResolver>(DependencyLifetime.Singleton, NonScoped);
        Register<IDependencyResolver, DependencyResolver>(DependencyLifetime.Transient, Scoped);
        Register<IObjectConstructor, ObjectConstructor>(DependencyLifetime.Singleton);
        Register<IResolvingObjectsService, ResolvingObjectsService>(DependencyLifetime.Singleton, NonScoped);
        Register<IResolvingObjectsService, ResolvingObjectsService>(DependencyLifetime.Transient, Scoped);
        Register<IScope, Scope>(DependencyLifetime.Transient);
    }

    /// <summary>
    /// Gets the unique key that identifies the particular container instance that this
    /// <see cref="ServiceLocator" /> instance belongs to.
    /// </summary>
    private string ContainerKey
    {
        get;
        init;
    }

    /// <summary>
    /// Get an instance of the implementing class that is mapped to the given interface type
    /// <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">
    /// The interface type for which we want to retrieve the corresponding implementation class
    /// object.
    /// </typeparam>
    /// <param name="resolvingKey">
    /// An optional key used to identify the specific implementation class object to be retrieved.
    /// </param>
    /// <returns>
    /// An instance of the implementation type that has been mapped to the given interface type
    /// <typeparamref name="T" />.
    /// </returns>
    /// <exception cref="ServiceLocatorException" />
    public T Get<T>(string resolvingKey = EmptyKey) where T : class
    {
        if (_services.TryGetValue(new(typeof(T), resolvingKey), out Dependency? service))
        {
            if (service is not null)
            {
                try
                {
                    ConstructorInfo? constructorInfo = GetConstructorInfo<T>(service, resolvingKey);

                    ServiceKey serviceKey = service.ResolvingServiceKey;

                    if (service.Lifetime is DependencyLifetime.Singleton)
                    {
                        return GetSingleton<T>(serviceKey, constructorInfo);
                    }

                    return CreateInstance<T>(constructorInfo, resolvingKey);
                }
                catch (Exception ex)
                {
                    // This exception should never happen. It's added here just in case.
                    string msg1 = FormatMessage<T>(MsgUnableToConstructService, resolvingKey);
                    throw new ServiceLocatorException(msg1, ex);
                }
            }
        }

        string msg2 = FormatMessage<T>(MsgServiceNotRegistered, resolvingKey);
        throw new ServiceLocatorException(msg2);
    }

    /// <summary>
    /// Retrieves an instance of <see cref="IServiceLocator" /> associated with the specified
    /// <paramref name="containerKey" />.
    /// </summary>
    /// <remarks>
    /// This method is thread-safe. If multiple threads attempt to retrieve an instance for the same
    /// container key simultaneously, only one instance will be created.
    /// </remarks>
    /// <param name="containerKey">
    /// The unique key identifying the service container.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IServiceLocator" /> associated with the specified container key.
    /// If no instance exists for the key, a new one is created and returned.
    /// </returns>
    internal static IServiceLocator GetInstance(string containerKey)
    {
        if (!_serviceLocators.ContainsKey(containerKey))
        {
            lock (_globalLock)
            {
                if (!_serviceLocators.ContainsKey(containerKey))
                {
                    _serviceLocators[containerKey] = new ServiceLocator(containerKey);
                }
            }
        }

        return _serviceLocators[containerKey];
    }

    /// <summary>
    /// Retrieves the constructor information for the specified type <typeparamref name="T" /> that
    /// matches the given parameters.
    /// </summary>
    /// <typeparam name="T">
    /// The type for which the constructor information is being retrieved. Must be a reference type.
    /// </typeparam>
    /// <param name="service">
    /// The dependency containing the type information to resolve the constructor.
    /// </param>
    /// <param name="resolvingKey">
    /// A key used to identify the specific service or context for the resolution.
    /// </param>
    /// <returns>
    /// The <see cref="ConstructorInfo" /> object representing the matching constructor.
    /// </returns>
    /// <exception cref="ServiceLocatorException">
    /// Thrown if a matching constructor cannot be found for the specified type
    /// <typeparamref name="T" />.
    /// </exception>
    private static ConstructorInfo GetConstructorInfo<T>(Dependency service, string resolvingKey) where T : class
    {
        ConstructorInfo? constructorInfo = service.ResolvingType.GetConstructor(ConstructorBindingFlags,
                                                                                null,
                                                                                [typeof(string)],
                                                                                null);

        if (constructorInfo is null)
        {
            // This exception should never happen.
            string msg = FormatMessage<T>(MsgUnableToObtainConstructor, resolvingKey);
            throw new ServiceLocatorException(msg);
        }

        return constructorInfo;
    }

    /// <summary>
    /// Creates an instance of the specified type using the provided constructor information.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the instance to create. Must be a reference type.
    /// </typeparam>
    /// <param name="constructorInfo">
    /// The <see cref="ConstructorInfo" /> used to invoke the constructor for creating the instance.
    /// </param>
    /// <param name="resolvingKey">
    /// A string key associated with the instance creation, used for error reporting.
    /// </param>
    /// <returns>
    /// An instance of type <typeparamref name="T" /> created using the specified constructor.
    /// </returns>
    /// <exception cref="ServiceLocatorException">
    /// Thrown if the constructor invocation does not produce an instance of type
    /// <typeparamref name="T" />.
    /// </exception>
    private T CreateInstance<T>(ConstructorInfo constructorInfo, string resolvingKey) where T : class
    {
        if (constructorInfo.Invoke([ContainerKey]) is not T instance)
        {
            // This exception should never happen.
            string msg = FormatMessage<T>(MsgNullInstanceCreated, resolvingKey);
            throw new ServiceLocatorException(msg);
        }

        return instance;
    }

    /// <summary>
    /// Retrieves a singleton instance of the specified type, creating it if it does not already
    /// exist.
    /// </summary>
    /// <remarks>
    /// This method ensures thread safety when creating singleton instances. If the instance does
    /// not already exist, it is created using the provided <paramref name="constructorInfo" /> and
    /// stored for future retrieval.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of the singleton instance to retrieve. Must be a reference type.
    /// </typeparam>
    /// <param name="serviceKey">
    /// The key used to uniquely identify the singleton instance.
    /// </param>
    /// <param name="constructorInfo">
    /// The constructor information used to create the instance if it does not already exist.
    /// </param>
    /// <returns>
    /// The singleton instance of type <typeparamref name="T" /> associated with the specified
    /// <paramref name="serviceKey" />.
    /// </returns>
    private T GetSingleton<T>(ServiceKey serviceKey, ConstructorInfo constructorInfo) where T : class
    {
        if (!_singletonInstances.ContainsKey(serviceKey))
        {
            lock (_localLock)
            {
                if (!_singletonInstances.ContainsKey(serviceKey))
                {
                    T instance = CreateInstance<T>(constructorInfo, serviceKey.ResolvingKey);
                    _singletonInstances[serviceKey] = instance;
                }
            }
        }

        return (T)_singletonInstances[serviceKey];
    }

    /// <summary>
    /// Register a dependency with the service locator.
    /// </summary>
    /// <typeparam name="TInterface">
    /// The interface type for the dependency being registered.
    /// </typeparam>
    /// <typeparam name="TImplementation">
    /// The implementation type for the dependency being registered.
    /// </typeparam>
    /// <param name="lifetime">
    /// The lifetime of the dependency being registered.
    /// </param>
    /// <param name="resolvingKey">
    /// An optional key used to identify the specific implementation class object to be retrieved.
    /// </param>
    /// <exception cref="ServiceLocatorException" />
    private void Register<TInterface, TImplementation>(DependencyLifetime lifetime, string resolvingKey = EmptyKey)
        where TInterface : class
        where TImplementation : TInterface
    {
        Dependency dependency = new(typeof(TInterface), typeof(TImplementation), lifetime, resolvingKey);
        ServiceKey serviceKey = dependency.DependencyServiceKey;

        if (!_services.ContainsKey(serviceKey))
        {
            lock (_localLock)
            {
                if (!_services.ContainsKey(serviceKey))
                {
                    _services[serviceKey] = dependency;
                    return;
                }
            }
        }

        // This exception should never happen.
        string msg = FormatMessage(MsgDuplicateService, typeof(TInterface), resolvingKey, typeof(TImplementation));
        throw new ServiceLocatorException(msg);
    }
}