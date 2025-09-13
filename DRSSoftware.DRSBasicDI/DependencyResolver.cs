namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Attributes;
using DRSSoftware.DRSBasicDI.Extensions;
using DRSSoftware.DRSBasicDI.Interfaces;
using System.Reflection;

/// <summary>
/// The <see cref="DependencyResolver" /> class is responsible for creating an instance of the
/// resolving object for a given dependency type.
/// </summary>
/// <remarks>
/// This class is instantiated only by the <see cref="ServiceLocator" /> class.
/// </remarks>
internal sealed class DependencyResolver : IDependencyResolver
{
    /// <summary>
    /// Save the <see cref="MethodInfo" /> details for the <see cref="Resolve{T}(string)" /> method
    /// so that we can dynamically invoke the method for different generic types.
    /// </summary>
    private readonly MethodInfo _resolveMethodInfo
        = typeof(DependencyResolver).GetMethod(nameof(Resolve),
                                               BindingFlags.Instance | BindingFlags.Public,
                                               null,
                                               [typeof(string)],
                                               null)
        ?? throw new DependencyInjectionException(MsgResolveMethodInfoNotFound);

    /// <summary>
    /// Flag that is set to <see langword="true" /> when the <see cref="DependencyResolver" /> has
    /// been disposed of.
    /// </summary>
    /// <remarks>
    /// This flag is used to prevent multiple calls to the <see cref="Dispose()" /> method from
    /// causing any unforeseen issues.
    /// </remarks>
    private bool _isDisposed;

    /// <summary>
    /// Create a new instance of the <see cref="DependencyResolver" /> class.
    /// </summary>
    /// <param name="containerKey">
    /// A <see langword="string" /> used to identify the specific <see cref="ServiceLocator" />
    /// instance to use in resolving dependencies.
    /// </param>
    private DependencyResolver(string containerKey) : this(ServiceLocator.GetInstance(containerKey))
    {
    }

    /// <summary>
    /// Create a new instance of the <see cref="DependencyResolver" /> class. This constructor is
    /// intended for use in unit testing only.
    /// </summary>
    /// <param name="serviceLocator">
    /// A service locator object that should provide mock instances of the requested dependencies.
    /// </param>
    internal DependencyResolver(IServiceLocator serviceLocator)
    {
        DependencyList = serviceLocator.Get<IDependencyListConsumer>();
        ObjectConstructor = serviceLocator.Get<IObjectConstructor>();
        NonScopedService = serviceLocator.Get<IResolvingObjectsService>(NonScoped);
    }

    /// <summary>
    /// Get a reference to the <see cref="IDependencyListConsumer" /> object.
    /// </summary>
    private IDependencyListConsumer DependencyList
    {
        get;
        init;
    }

    /// <summary>
    /// Get a reference to the <see cref="IResolvingObjectsService" /> instance used for managing
    /// non-scoped dependency objects.
    /// </summary>
    private IResolvingObjectsService NonScopedService
    {
        get;
        init;
    }

    /// <summary>
    /// Get a reference to the <see cref="IObjectConstructor" /> object.
    /// </summary>
    private IObjectConstructor ObjectConstructor
    {
        get;
        init;
    }

    /// <summary>
    /// Get a reference to the <see cref="IResolvingObjectsService" /> instance used for managing
    /// scoped dependency objects.
    /// </summary>
    private IResolvingObjectsService? ScopedService
    {
        get;
        set;
    }

    /// <summary>
    /// Call the <see cref="ResolvingObjectsService.Dispose()" /> method on the
    /// <see cref="ScopedService" /> instance if it isn't null. Otherwise, call the method on the
    /// <see cref="NonScopedService" /> instance.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed is false)
        {
            if (ScopedService is not null)
            {
                ScopedService.Dispose();
            }
            else
            {
                NonScopedService.Dispose();
            }

            _isDisposed = true;
        }
    }

    /// <summary>
    /// Retrieve the resolving object for the given dependency type
    /// <typeparamref name="TDependency" />.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type that is to be resolved.
    /// </typeparam>
    /// <param name="resolvingKey">
    /// An optional key used to identify the specific resolving object to be retrieved.
    /// </param>
    /// <returns>
    /// An instance of the resolving class type cast as the dependency type.
    /// </returns>
    /// <remarks>
    /// This method will be called recursively until all nested constructor dependencies have been
    /// resolved.
    /// </remarks>
    /// <exception cref="DependencyInjectionException" />
    public TDependency Resolve<TDependency>(string resolvingKey) where TDependency : class
    {
        if (TryGetResolvedDependency(out TDependency? resolvedDependency, resolvingKey))
        {
            // The only way we will get to this point is if the resolvedDependency value is not
            // null.
            return resolvedDependency!;
        }

        return ConstructResolvingInstance<TDependency>(resolvingKey);
    }

    /// <summary>
    /// Retrieve the resolving object for the given dependency type
    /// <typeparamref name="TDependency" />. The object will be constructed using the given
    /// constructor <paramref name="parameters" />.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type that is to be resolved.
    /// </typeparam>
    /// <param name="parameters">
    /// The list of constructor parameters used for constructing the resolving object.
    /// </param>
    /// <param name="resolvingKey">
    /// An optional key used to identify the specific resolving object to be retrieved.
    /// </param>
    /// <returns>
    /// An instance of the resolving class type cast as the dependency type.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    public TDependency Resolve<TDependency>(object[] parameters, string resolvingKey) where TDependency : class
    {
        ArgumentNullException.ThrowIfNull(parameters, nameof(parameters));

        if (TryGetResolvedDependency(out TDependency? resolvedDependency, resolvingKey))
        {
            // The only way we will get to this point is if the resolvedDependency value is not
            // null.
            return resolvedDependency!;
        }

        return ConstructResolvingInstance<TDependency>(parameters, resolvingKey);
    }

    /// <summary>
    /// Set the scoped <see cref="IResolvingObjectsService" /> to the specified
    /// <paramref name="resolvingObjectsService" /> instance.
    /// </summary>
    /// <param name="resolvingObjectsService">
    /// The scoped <see cref="IResolvingObjectsService" /> instance to be set.
    /// </param>
    /// <exception cref="DependencyInjectionException" />
    public void SetScopedService(IResolvingObjectsService resolvingObjectsService)
    {
        if (resolvingObjectsService is null)
        {
            throw new DependencyInjectionException(MsgScopedServiceIsNull);
        }

        if (ScopedService is null)
        {
            if (ReferenceEquals(resolvingObjectsService, NonScopedService))
            {
                throw new DependencyInjectionException(MsgScopedServiceSameAsNonScopedService);
            }

            ScopedService = resolvingObjectsService;
        }
        else
        {
            throw new DependencyInjectionException(MsgScopedServiceAlreadySet);
        }
    }

    /// <summary>
    /// Construct the resolving object for the given dependency type
    /// <typeparamref name="TDependency" />.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type whose resolving object is being constructed.
    /// </typeparam>
    /// <param name="resolvingKey">
    /// An optional key used to identify the specific resolving object to be constructed.
    /// </param>
    /// <returns>
    /// An instance of the resolving object for the given dependency type
    /// <typeparamref name="TDependency" />.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    private TDependency ConstructResolvingInstance<TDependency>(string resolvingKey) where TDependency : class
    {
        ConstructorInfo constructorInfo = GetDIConstructorInfo<TDependency>(resolvingKey);
        object[] resolvedParameters = ResolveNestedDependencies(constructorInfo);
        TDependency resolvingObject = ObjectConstructor.Construct<TDependency>(constructorInfo, resolvedParameters, resolvingKey);
        return SaveResolvedDependency(resolvingObject, resolvingKey);
    }

    /// <summary>
    /// Construct the resolving object for the given dependency type
    /// <typeparamref name="TDependency" /> using the specified constructor
    /// <paramref name="parameters" />.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type whose resolving object is being constructed.
    /// </typeparam>
    /// <param name="parameters">
    /// The list of constructor parameters to be used in constructing the resolving object.
    /// </param>
    /// <param name="resolvingKey">
    /// An optional key used to identify the specific resolving object to be constructed.
    /// </param>
    /// <returns>
    /// An instance of the resolving object for the given dependency type
    /// <typeparamref name="TDependency" />.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    private TDependency ConstructResolvingInstance<TDependency>(object[] parameters, string resolvingKey) where TDependency : class
    {
        ConstructorInfo constructorInfo = GetDIConstructorInfo<TDependency>(resolvingKey, parameters.Length);
        TDependency resolvingObject = ObjectConstructor.Construct<TDependency>(constructorInfo, parameters, resolvingKey);
        return SaveResolvedDependency(resolvingObject, resolvingKey);
    }

    /// <summary>
    /// Retrieves the constructor information for a dependency type, optionally filtered by the
    /// number of parameters.
    /// </summary>
    /// <remarks>
    /// This method uses the dependency list to resolve the type associated with the specified key
    /// and retrieves the constructor information for that type. If
    /// <paramref name="parameterCount" /> is specified, the method attempts to find a constructor
    /// with the exact number of parameters.
    /// </remarks>
    /// <typeparam name="TDependency">
    /// The type of the dependency to resolve. Must be a class.
    /// </typeparam>
    /// <param name="resolvingKey">
    /// The key used to identify the dependency in the dependency list.
    /// </param>
    /// <param name="parameterCount">
    /// The number of parameters expected in the constructor. If set to 0, the constructor having
    /// the most constructor parameters is retrieved.
    /// </param>
    /// <returns>
    /// A <see cref="ConstructorInfo" /> object representing the resolved constructor for the
    /// specified dependency type.
    /// </returns>
    private ConstructorInfo GetDIConstructorInfo<TDependency>(string resolvingKey, int parameterCount = 0) where TDependency : class
    {
        IDependency dependency = DependencyList.Get<TDependency>(resolvingKey);
        Type resolvingType = dependency.ResolvingType;

        return parameterCount == 0
            ? resolvingType.GetDIConstructorInfo()
            : resolvingType.GetDIConstructorInfo(parameterCount);
    }

    /// <summary>
    /// Gets the resolving objects corresponding to each of the parameters in the constructor that
    /// is identified by the <paramref name="constructorInfo" /> parameter.
    /// </summary>
    /// <param name="constructorInfo">
    /// The <see cref="ConstructorInfo" /> of a resolving object for some dependency type.
    /// </param>
    /// <returns>
    /// An array of resolving objects corresponding to the parameters of the given
    /// <paramref name="constructorInfo" />.
    /// </returns>
    /// <remarks>
    /// This method makes recursive calls to <see cref="Resolve{TDependency}(string)" /> until all
    /// nested dependencies have been resolved.
    /// </remarks>
    /// <exception cref="DependencyInjectionException" />
    private object[] ResolveNestedDependencies(ConstructorInfo constructorInfo)
    {
        ParameterInfo[] parameters = constructorInfo.GetParameters();
        List<object> resolvedParameters = [];

        foreach (ParameterInfo parameter in parameters)
        {
            ResolvingKeyAttribute? attribute = parameter.GetCustomAttribute<ResolvingKeyAttribute>();
            string resolvingKey = attribute?.Value ?? EmptyKey;
            Type parameterType = parameter.ParameterType;
            MethodInfo resolveMethodInfo;

            try
            {
                // Create a generic version of the Resolve<T>() method using the current parameter
                // type as the generic type parameter T.
                resolveMethodInfo = _resolveMethodInfo.MakeGenericMethod(parameterType);
            }
            catch (Exception ex)
            {
                string msg = FormatMessage(MsgUnableToMakeGenericResolveMethod, parameterType, resolvingKey);
                throw new DependencyInjectionException(msg, ex);
            }

            object? resolvedParameter;

            try
            {
                // Invoke the generic Resolve<T>() method for the current parameter type.
                resolvedParameter = resolveMethodInfo.Invoke(this, [resolvingKey]);
            }
            catch (Exception ex)
            {
                string msg = FormatMessage(MsgResolveMethodInvocationError, parameterType, resolvingKey);
                throw new DependencyInjectionException(msg, ex);
            }

            // We should not get a null value returned from the generic Resolve<T>() method.
            if (resolvedParameter is null)
            {
                string msg = FormatMessage(MsgResolvingObjectCouldNotBeCreated, parameterType, resolvingKey);
                throw new DependencyInjectionException(msg);
            }

            resolvedParameters.Add(resolvedParameter);
        }

        return [.. resolvedParameters];
    }

    /// <summary>
    /// Save the given resolved dependency object for later use if applicable.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of the dependency that was resolved.
    /// </typeparam>
    /// <param name="resolvedDependency">
    /// An instance of the resolving type that was mapped to the dependency type
    /// <typeparamref name="TDependency" />.
    /// </param>
    /// <param name="resolvingKey">
    /// An optional key used to identify the specific resolving object to be saved.
    /// </param>
    /// <returns>
    /// The <paramref name="resolvedDependency" /> that was passed in, or the resolving instance
    /// that was previously saved by another application thread.
    /// </returns>
    /// <remarks>
    /// Only scoped and singleton dependencies are saved. Transient dependencies by definition are
    /// created new every time they're requested.
    /// </remarks>
    private TDependency SaveResolvedDependency<TDependency>(TDependency resolvedDependency, string resolvingKey) where TDependency : class
    {
        IDependency dependency = DependencyList.Get<TDependency>(resolvingKey);

        if (dependency.Lifetime is DependencyLifetime.Scoped && ScopedService is not null)
        {
            return ScopedService.Add(resolvedDependency, resolvingKey);
        }
        else if (dependency.Lifetime is DependencyLifetime.Scoped or DependencyLifetime.Singleton)
        {
            return NonScopedService.Add(resolvedDependency, resolvingKey);
        }

        return resolvedDependency;
    }

    /// <summary>
    /// Try to retrieve the resolving object for the given dependency type if one was previously
    /// saved.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of the dependency that is being resolved.
    /// </typeparam>
    /// <param name="resolvedDependency">
    /// The resolving object that is returned for the given dependency type
    /// <typeparamref name="TDependency" /> if one was previously saved. Otherwise,
    /// <see langword="null" />.
    /// </param>
    /// <param name="resolvingKey">
    /// An optional key used to identify the specific resolving object to be retrieved.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if a valid resolving object is successfully retrieved. Otherwise,
    /// <see langword="false" />.
    /// </returns>
    private bool TryGetResolvedDependency<TDependency>(out TDependency? resolvedDependency, string resolvingKey) where TDependency : class
    {
        IDependency dependency = DependencyList.Get<TDependency>(resolvingKey);

        if (ScopedService is not null && dependency.Lifetime is DependencyLifetime.Scoped)
        {
            return ScopedService.TryGetResolvingObject(out resolvedDependency, dependency);
        }

        if (dependency.Lifetime is DependencyLifetime.Singleton or DependencyLifetime.Scoped)
        {
            return NonScopedService.TryGetResolvingObject(out resolvedDependency, dependency);
        }

        resolvedDependency = null;
        return false;
    }
}