namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;
using System.Reflection;
using IContainer = Interfaces.IContainer;

/// <summary>
/// The <see cref="ServiceLocater" /> class implements a basic service locater pattern for
/// dependency injection. This class is used strictly for resolving dependencies within the
/// <see cref="DrsBasicDI" /> class library.
/// </summary>
internal sealed class ServiceLocater : IServiceLocater
{
    /// <summary>
    /// The <see cref="BindingFlags" /> used to find constructors for the implementation types.
    /// </summary>
    private const BindingFlags ConstructorBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// A lazy initializer for the <see cref="ServiceLocater" /> class that creates a singleton
    /// instance of the class.
    /// </summary>
    private static readonly Lazy<IServiceLocater> _instance = new(() => new ServiceLocater());

    /// <summary>
    /// A dictionary of service descriptors used to store the interface type, implementation type,
    /// and lifetime of all dependencies in the <see cref="DrsBasicDI" /> class library.
    /// </summary>
    private readonly Dictionary<ServiceKey, ServiceDescriptor> _serviceDescriptors = [];

    /// <summary>
    /// A dictionary of singleton instances used to store all singleton dependency objects in the
    /// <see cref="DrsBasicDI" /> class library.
    /// </summary>
    private readonly Dictionary<ServiceKey, object> _singletonInstances = [];

    /// <summary>
    /// Create a new instance of the <see cref="ServiceLocater" /> class and register all of the
    /// dependencies for the <see cref="DrsBasicDI" /> class library.
    /// </summary>
    /// <remarks>
    /// This is a private constructor to prevent external instantiation of the
    /// <see cref="ServiceLocater" /> class. The static <see cref="Instance" /> property must be
    /// used to obtain an instance of the <see cref="ServiceLocater" /> class.
    /// </remarks>
    private ServiceLocater()
    {
        RegisterSingleton<IContainer, Container>();
        RegisterSingleton<IDependencyListBuilder, DependencyList>();
        RegisterSingleton<IDependencyListConsumer, DependencyList>();
        RegisterSingleton<IDependencyResolver, DependencyResolver>(NonScoped);
        RegisterTransient<IDependencyResolver, DependencyResolver>(Scoped);
        RegisterSingleton<IObjectConstructor, ObjectConstructor>();
        RegisterSingleton<IResolvingObjectsService, ResolvingObjectsService>(NonScoped);
        RegisterTransient<IResolvingObjectsService, ResolvingObjectsService>(Scoped);
        RegisterTransient<IScope, Scope>();
    }

    /// <summary>
    /// Get the singleton instance of the <see cref="ServiceLocater" /> class.
    /// </summary>
    internal static IServiceLocater Instance => _instance.Value;

    /// <summary>
    /// Get an instance of the implementing class that is mapped to the given interface type
    /// <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">
    /// The interface type for which we want to retrieve the corresponding implementation class
    /// object.
    /// </typeparam>
    /// <param name="key">
    /// An optional key used to identify the specific implementation class object to be retrieved.
    /// </param>
    /// <returns>
    /// An instance of the implementation type that has been mapped to the given interface type
    /// <typeparamref name="T" />.
    /// </returns>
    /// <exception cref="ServiceLocaterException" />
    public T Get<T>(string key = EmptyKey) where T : class
    {
        ServiceKey serviceKey = ServiceKey.GetServiceKey<T>(key);

        if (_serviceDescriptors.TryGetValue(serviceKey, out ServiceDescriptor? serviceDescriptor))
        {
            if (serviceDescriptor is not null)
            {
                try
                {
                    serviceKey = ServiceKey.GetServiceKey(serviceDescriptor.ImplementationType, key);
                    ConstructorInfo? constructorInfo = serviceDescriptor.ImplementationType.GetConstructor(ConstructorBindingFlags,
                                                                                                           null,
                                                                                                           [],
                                                                                                           null);

                    if (serviceDescriptor.Lifetime == DependencyLifetime.Singleton)
                    {
                        if (_singletonInstances.TryGetValue(serviceKey, out object? instance))
                        {
                            if (instance is not null)
                            {
                                return (T)instance;
                            }
                        }

                        if (constructorInfo is not null)
                        {
                            instance = constructorInfo.Invoke([]);
                            _singletonInstances[serviceKey] = instance;
                            return (T)instance;
                        }
                    }

                    if (constructorInfo is not null)
                    {
                        return (T)constructorInfo.Invoke(null);
                    }
                }
                catch (Exception ex)
                {
                    string msg1 = FormatMessage<T>(MsgUnableToConstructService, key);
                    throw new ServiceLocaterException(msg1, ex);
                }

                string msg2 = FormatMessage<T>(MsgUnableToConstructService, key);
                throw new ServiceLocaterException(msg2);
            }
        }

        string msg3 = FormatMessage<T>(MsgServiceNotRegistered, key);
        throw new ServiceLocaterException(msg3);
    }

    /// <summary>
    /// Register a singleton dependency with the service locater.
    /// </summary>
    /// <typeparam name="TInterface">
    /// The interface type for the dependency being registered.
    /// </typeparam>
    /// <typeparam name="TImplementation">
    /// The implementation type for the dependency being registered.
    /// </typeparam>
    /// <param name="key">
    /// An optional key used to identify the specific implementation class object to be retrieved.
    /// </param>
    private void RegisterSingleton<TInterface, TImplementation>(string key = EmptyKey)
        where TInterface : class
        where TImplementation : TInterface
    {
        ServiceKey serviceKey = ServiceKey.GetServiceKey<TInterface>(key);
        _serviceDescriptors[serviceKey] = new ServiceDescriptor(typeof(TInterface),
                                                                typeof(TImplementation),
                                                                DependencyLifetime.Singleton);
    }

    /// <summary>
    /// Register a transient dependency with the service locater.
    /// </summary>
    /// <typeparam name="TInterface">
    /// The interface type for the dependency being registered.
    /// </typeparam>
    /// <typeparam name="TImplementation">
    /// The implementation type for the dependency being registered.
    /// </typeparam>
    /// <param name="key">
    /// An optional key used to identify the specific implementation class object to be retrieved.
    /// </param>
    private void RegisterTransient<TInterface, TImplementation>(string key = EmptyKey)
        where TInterface : class
        where TImplementation : TInterface
    {
        ServiceKey serviceKey = ServiceKey.GetServiceKey<TInterface>(key);
        _serviceDescriptors[serviceKey] = new ServiceDescriptor(typeof(TInterface),
                                                                typeof(TImplementation),
                                                                DependencyLifetime.Transient);
    }

    /// <summary>
    /// The <see cref="ServiceDescriptor" /> class is used to store the interface type,
    /// implementation type, and lifetime of a dependency.
    /// </summary>
    /// <param name="InterfaceType">
    /// The interface type of the dependency.
    /// </param>
    /// <param name="ImplementationType">
    /// The implementation type of the dependency.
    /// </param>
    /// <param name="Lifetime">
    /// The lifetime of the dependency.
    /// </param>
    private sealed record ServiceDescriptor(Type InterfaceType,
                                            Type ImplementationType,
                                            DependencyLifetime Lifetime)
    {
    }
}