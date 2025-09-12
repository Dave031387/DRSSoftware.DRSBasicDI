namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="ContainerBuilder" /> class is used for constructing a new
/// <see cref="Container" /> object to be used for dependency injection.
/// </summary>
/// <remarks>
/// Once the <see cref="Build" /> method has been called, any other method calls other than
/// <see cref="GetTestInstance(IServiceLocator)" /> will result in a
/// <see cref="ContainerBuildException" /> being thrown.
/// </remarks>
public sealed class ContainerBuilder : IContainerBuilder
{
    /// <summary>
    /// A dictionary of <see cref="IContainerBuilder" /> objects.
    /// </summary>
    private static readonly Dictionary<string, IContainerBuilder> _builders = [];

    /// <summary>
    /// A lock object used to ensure thread safety.
    /// </summary>
    private static readonly object _lock = new();

    /// <summary>
    /// Represents the count of test containers currently in use.
    /// </summary>
    /// <remarks>
    /// This field is used to track the number of test containers. It is intended for internal use
    /// only.
    /// </remarks>
    private static int _testContainerCount;

    /// <summary>
    /// A boolean flag that gets set to <see langword="true" /> once the <see cref="IContainer" />
    /// object has been built.
    /// </summary>
    private bool _containerHasBeenBuilt;

    /// <summary>
    /// Default constructor for the <see cref="ContainerBuilder" /> class.
    /// </summary>
    /// <param name="containerKey">
    /// An optional key for identifying the specific instance of the <see cref="ContainerBuilder" />
    /// that is being used.
    /// </param>
    /// <remarks>
    /// This constructor is declared <see langword="private" />. Use the static
    /// <see cref="GetInstance(string)" /> method to create a new, empty
    /// <see cref="ContainerBuilder" /> object.
    /// </remarks>
    private ContainerBuilder(string containerKey)
        : this(DRSBasicDI.ServiceLocator.GetInstance(containerKey), containerKey)
    {
    }

    /// <summary>
    /// Create a new instance of the <see cref="ContainerBuilder" /> class using the specified
    /// <paramref name="serviceLocator" /> object and <paramref name="containerKey" />. This
    /// constructor is intended for unit testing only.
    /// </summary>
    /// <param name="serviceLocator">
    /// A service locator object that should provide mock instances of the requested dependencies.
    /// </param>
    /// <param name="containerKey">
    /// An optional key for identifying the specific instance of the <see cref="ContainerBuilder" />
    /// that is being used.
    /// </param>
    /// <remarks>
    /// This constructor is declared <see langword="private" />. Use the static
    /// <see cref="GetTestInstance(IServiceLocator)" /> method to create a new, empty
    /// <see cref="IContainerBuilder" /> object for testing purposes.
    /// </remarks>
    private ContainerBuilder(IServiceLocator serviceLocator, string containerKey)
    {
        ServiceLocator = serviceLocator;
        DependencyList = ServiceLocator.Get<IDependencyListBuilder>();
        ContainerKey = containerKey;
    }

    /// <summary>
    /// Gets the key value used to identify this specific <see cref="ContainerBuilder" /> instance.
    /// </summary>
    public string ContainerKey
    {
        get;
        private init;
    }

    /// <summary>
    /// Get a reference to the <see cref="IDependencyListBuilder" /> object.
    /// </summary>
    private IDependencyListBuilder DependencyList
    {
        get;
        init;
    }

    /// <summary>
    /// Get a reference to the <see cref="IServiceLocator" /> object.
    /// </summary>
    private IServiceLocator ServiceLocator
    {
        get;
        init;
    }

    /// <summary>
    /// Retrieves an instance of <see cref="IContainerBuilder" /> associated with the specified
    /// <paramref name="containerKey" />.
    /// </summary>
    /// <remarks>
    /// This method is thread-safe. If multiple threads attempt to retrieve an instance for the same
    /// <paramref name="containerKey" /> simultaneously, only one instance will be created.
    /// </remarks>
    /// <param name="containerKey">
    /// The key used to identify the <see cref="IContainerBuilder" /> instance. If not provided, a
    /// default key is used.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IContainerBuilder" /> associated with the specified key. If no
    /// instance exists for the key, a new instance is created and returned.
    /// </returns>
    /// <exception cref="ContainerBuildException">
    /// An exception is thrown if the <paramref name="containerKey" /> value is
    /// <see langword="null" />.
    /// </exception>
    public static IContainerBuilder GetInstance(string containerKey = EmptyKey)
    {
        if (containerKey is null)
        {
            throw new ContainerBuildException(MsgNullContainerKey);
        }

        if (!_builders.ContainsKey(containerKey))
        {
            lock (_lock)
            {
                if (!_builders.ContainsKey(containerKey))
                {
                    _builders[containerKey] = new ContainerBuilder(containerKey);
                }
            }
        }

        return _builders[containerKey];
    }

    /// <summary>
    /// Construct a new <see cref="IDependency" /> object and add it to the container.
    /// </summary>
    /// <param name="builder">
    /// A builder function that takes a <see cref="DependencyBuilder" /> object as input and returns
    /// the updated <see cref="DependencyBuilder" /> object.
    /// </param>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddDependency(Func<DependencyBuilder, DependencyBuilder> builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        FinalizeDependency(builder(DependencyBuilder.CreateNew));
        return this;
    }

    /// <summary>
    /// Construct a new <see cref="IDependency" /> object having the specified dependency type
    /// <typeparamref name="TDependency" /> and add it to the container.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of the dependency.
    /// </typeparam>
    /// <param name="builder">
    /// A builder function that takes a <see cref="DependencyBuilder" /> object as input and returns
    /// the updated <see cref="DependencyBuilder" /> object.
    /// </param>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddDependency<TDependency>(Func<DependencyBuilder, DependencyBuilder> builder) where TDependency : class
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        FinalizeDependency(builder(DependencyBuilder.CreateNew)
            .WithDependencyType<TDependency>());
        return this;
    }

    /// <summary>
    /// Construct a new <see cref="IDependency" /> object having the specified dependency type
    /// <typeparamref name="TDependency" /> and resolving type <typeparamref name="TResolving" />
    /// and add it to the container.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of the dependency.
    /// </typeparam>
    /// <typeparam name="TResolving">
    /// The resolving type that is mapped to the dependency type
    /// <typeparamref name="TDependency" />.
    /// </typeparam>
    /// <param name="builder">
    /// A builder function that takes a <see cref="DependencyBuilder" /> object as input and returns
    /// the updated <see cref="DependencyBuilder" /> object.
    /// </param>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddDependency<TDependency, TResolving>(Func<DependencyBuilder, DependencyBuilder> builder)
        where TDependency : class
        where TResolving : class, TDependency
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        FinalizeDependency(builder(DependencyBuilder.CreateNew)
            .WithDependencyType<TDependency>()
            .WithResolvingType<TResolving>());
        return this;
    }

    /// <summary>
    /// Construct a new scoped <see cref="IDependency" /> object and add it to the container.
    /// </summary>
    /// <param name="builder">
    /// A builder function that takes a <see cref="DependencyBuilder" /> object as input and returns
    /// the updated <see cref="DependencyBuilder" /> object.
    /// </param>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddScoped(Func<DependencyBuilder, DependencyBuilder> builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        FinalizeDependency(builder(DependencyBuilder.CreateNew)
            .WithLifetime(DependencyLifetime.Scoped));
        return this;
    }

    /// <summary>
    /// Construct a new scoped <see cref="IDependency" /> object having the specified dependency
    /// type <typeparamref name="TDependency" /> and add it to the container.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of the dependency.
    /// </typeparam>
    /// <param name="builder">
    /// A builder function that takes a <see cref="DependencyBuilder" /> object as input and returns
    /// the updated <see cref="DependencyBuilder" /> object.
    /// </param>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddScoped<TDependency>(Func<DependencyBuilder, DependencyBuilder> builder) where TDependency : class
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        FinalizeDependency(builder(DependencyBuilder.CreateNew)
            .WithDependencyType<TDependency>()
            .WithLifetime(DependencyLifetime.Scoped));
        return this;
    }

    /// <summary>
    /// Construct a new scoped <see cref="IDependency" /> object having the specified dependency
    /// type <typeparamref name="TDependency" /> and resolving type
    /// <typeparamref name="TResolving" /> and add it to the container.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of the dependency.
    /// </typeparam>
    /// <typeparam name="TResolving">
    /// The resolving type that is mapped to the given dependency type
    /// <typeparamref name="TDependency" />
    /// </typeparam>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddScoped<TDependency, TResolving>()
        where TDependency : class
        where TResolving : class, TDependency
    {
        FinalizeDependency(DependencyBuilder
            .CreateNew
            .WithDependencyType<TDependency>()
            .WithResolvingType<TResolving>()
            .WithLifetime(DependencyLifetime.Scoped));
        return this;
    }

    /// <summary>
    /// Construct a new scoped <see cref="IDependency" /> object having the specified dependency
    /// type <typeparamref name="TDependency" /> and resolving type
    /// <typeparamref name="TResolving" /> and add it to the container.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of the dependency.
    /// </typeparam>
    /// <typeparam name="TResolving">
    /// The resolving type that is mapped to the given dependency type
    /// <typeparamref name="TDependency" />
    /// </typeparam>
    /// <param name="builder">
    /// A builder function that takes a <see cref="DependencyBuilder" /> object as input and returns
    /// the updated <see cref="DependencyBuilder" /> object.
    /// </param>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddScoped<TDependency, TResolving>(Func<DependencyBuilder, DependencyBuilder> builder)
        where TDependency : class
        where TResolving : class, TDependency
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        FinalizeDependency(builder(DependencyBuilder.CreateNew)
            .WithDependencyType<TDependency>()
            .WithResolvingType<TResolving>()
            .WithLifetime(DependencyLifetime.Scoped));
        return this;
    }

    /// <summary>
    /// Construct a new singleton <see cref="IDependency" /> object and add it to the container.
    /// </summary>
    /// <param name="builder">
    /// A builder function that takes a <see cref="DependencyBuilder" /> object as input and returns
    /// the updated <see cref="DependencyBuilder" /> object.
    /// </param>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddSingleton(Func<DependencyBuilder, DependencyBuilder> builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        FinalizeDependency(builder(DependencyBuilder.CreateNew)
            .WithLifetime(DependencyLifetime.Singleton));
        return this;
    }

    /// <summary>
    /// Construct a new singleton <see cref="IDependency" /> object having the specified dependency
    /// type <typeparamref name="TDependency" /> and add it to the container.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of the dependency.
    /// </typeparam>
    /// <param name="builder">
    /// A builder function that takes a <see cref="DependencyBuilder" /> object as input and returns
    /// the updated <see cref="DependencyBuilder" /> object.
    /// </param>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddSingleton<TDependency>(Func<DependencyBuilder, DependencyBuilder> builder) where TDependency : class
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        FinalizeDependency(builder(DependencyBuilder.CreateNew)
            .WithDependencyType<TDependency>()
            .WithLifetime(DependencyLifetime.Singleton));
        return this;
    }

    /// <summary>
    /// Construct a new singleton <see cref="IDependency" /> object having the specified dependency
    /// type <typeparamref name="TDependency" /> and resolving type
    /// <typeparamref name="TResolving" /> and add it to the container.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of the dependency.
    /// </typeparam>
    /// <typeparam name="TResolving">
    /// The resolving type that is mapped to the given dependency type
    /// <typeparamref name="TDependency" />.
    /// </typeparam>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddSingleton<TDependency, TResolving>()
        where TDependency : class
        where TResolving : class, TDependency
    {
        FinalizeDependency(DependencyBuilder
            .CreateNew
            .WithDependencyType<TDependency>()
            .WithResolvingType<TResolving>()
            .WithLifetime(DependencyLifetime.Singleton));
        return this;
    }

    /// <summary>
    /// Construct a new singleton <see cref="IDependency" /> object having the specified dependency
    /// type <typeparamref name="TDependency" /> and resolving type
    /// <typeparamref name="TResolving" /> and add it to the container.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of the dependency.
    /// </typeparam>
    /// <typeparam name="TResolving">
    /// The resolving type that is mapped to the given dependency type
    /// <typeparamref name="TDependency" />.
    /// </typeparam>
    /// <param name="builder">
    /// A builder function that takes a <see cref="DependencyBuilder" /> object as input and returns
    /// the updated <see cref="DependencyBuilder" /> object.
    /// </param>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddSingleton<TDependency, TResolving>(Func<DependencyBuilder, DependencyBuilder> builder)
        where TDependency : class
        where TResolving : class, TDependency
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        FinalizeDependency(builder(DependencyBuilder.CreateNew)
            .WithDependencyType<TDependency>()
            .WithResolvingType<TResolving>()
            .WithLifetime(DependencyLifetime.Singleton));
        return this;
    }

    /// <summary>
    /// Construct a new transient <see cref="IDependency" /> object and add it to the container.
    /// </summary>
    /// <param name="builder">
    /// A builder function that takes a <see cref="DependencyBuilder" /> object as input and returns
    /// the updated <see cref="DependencyBuilder" /> object.
    /// </param>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddTransient(Func<DependencyBuilder, DependencyBuilder> builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        FinalizeDependency(builder(DependencyBuilder.CreateNew)
            .WithLifetime(DependencyLifetime.Transient));
        return this;
    }

    /// <summary>
    /// Construct a new transient <see cref="IDependency" /> object having the specified dependency
    /// type <typeparamref name="TDependency" /> and add it to the container.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of the dependency.
    /// </typeparam>
    /// <param name="builder">
    /// A builder function that takes a <see cref="DependencyBuilder" /> object as input and returns
    /// the updated <see cref="DependencyBuilder" /> object.
    /// </param>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddTransient<TDependency>(Func<DependencyBuilder, DependencyBuilder> builder) where TDependency : class
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        FinalizeDependency(builder(DependencyBuilder.CreateNew)
            .WithDependencyType<TDependency>()
            .WithLifetime(DependencyLifetime.Transient));
        return this;
    }

    /// <summary>
    /// Construct a new transient <see cref="IDependency" /> object having the specified dependency
    /// type <typeparamref name="TDependency" /> and resolving type
    /// <typeparamref name="TResolving" /> and add it to the container.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of the dependency.
    /// </typeparam>
    /// <typeparam name="TResolving">
    /// The resolving type that is mapped to the given dependency type
    /// <typeparamref name="TDependency" />.
    /// </typeparam>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddTransient<TDependency, TResolving>()
        where TDependency : class
        where TResolving : class, TDependency
    {
        FinalizeDependency(DependencyBuilder
            .CreateNew
            .WithDependencyType<TDependency>()
            .WithResolvingType<TResolving>()
            .WithLifetime(DependencyLifetime.Transient));
        return this;
    }

    /// <summary>
    /// Construct a new transient <see cref="IDependency" /> object having the specified dependency
    /// type <typeparamref name="TDependency" /> and resolving type
    /// <typeparamref name="TResolving" /> and add it to the container.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of the dependency.
    /// </typeparam>
    /// <typeparam name="TResolving">
    /// The resolving type that is mapped to the given dependency type
    /// <typeparamref name="TDependency" />.
    /// </typeparam>
    /// <param name="builder">
    /// A builder function that takes a <see cref="DependencyBuilder" /> object as input and returns
    /// the updated <see cref="DependencyBuilder" /> object.
    /// </param>
    /// <returns>
    /// The updated <see cref="IContainerBuilder" /> object.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="ContainerBuildException" />
    public IContainerBuilder AddTransient<TDependency, TResolving>(Func<DependencyBuilder, DependencyBuilder> builder)
        where TDependency : class
        where TResolving : class, TDependency
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        FinalizeDependency(builder(DependencyBuilder.CreateNew)
            .WithDependencyType<TDependency>()
            .WithResolvingType<TResolving>()
            .WithLifetime(DependencyLifetime.Transient));
        return this;
    }

    /// <summary>
    /// Build the <see cref="IContainer" /> object.
    /// </summary>
    /// <returns>
    /// A new <see cref="IContainer" /> object containing all the <see cref="IDependency" /> objects
    /// that were added.
    /// </returns>
    /// <exception cref="ContainerBuildException" />
    public IContainer Build()
    {
        CheckForContainerAlreadyBuilt(true);

        // The DependencyList will always contain at least one entry since an entry is automatically
        // added for the IContainer dependency representing the dependency injection container
        // itself. Therefore it doesn't make sense to build the dependency injection container
        // unless the DependencyList contains at least two entries.
        if (DependencyList.Count < 2)
        {
            throw new ContainerBuildException(MsgContainerIsEmpty);
        }

        IContainer container = ServiceLocator.Get<IContainer>();
        _containerHasBeenBuilt = true;
        return container;
    }

    /// <summary>
    /// Get a new test instance of the <see cref="IContainerBuilder" /> object.
    /// </summary>
    /// <param name="serviceLocator">
    /// A service locator object that should provide mock instances of the requested dependencies.
    /// </param>
    /// <remarks>
    /// Please note that this method doesn't return a singleton instance. Each call to this method
    /// will return a new <see cref="IContainerBuilder" /> object.
    /// </remarks>
    /// <returns>
    /// An instance of the <see cref="IContainerBuilder" /> object to be used for unit testing.
    /// </returns>
    internal static IContainerBuilder GetTestInstance(IServiceLocator serviceLocator)
    {
        lock (_lock)
        {
            string containerKey = $"ContainerBuilderTest_{++_testContainerCount}";
            return new ContainerBuilder(serviceLocator, containerKey);
        }
    }

    /// <summary>
    /// Check to see if the <see cref="IContainer" /> object has already been built. Throw an
    /// appropriate exception if it has.
    /// </summary>
    /// <param name="isBuildAction">
    /// A boolean flag indicating whether or not this method is being called from the
    /// <see cref="Build" /> method.
    /// </param>
    /// <exception cref="ContainerBuildException" />
    private void CheckForContainerAlreadyBuilt(bool isBuildAction = false)
    {
        if (_containerHasBeenBuilt)
        {
            if (isBuildAction)
            {
                throw new ContainerBuildException(MsgContainerCantBeBuiltMoreThanOnce);
            }
            else
            {
                throw new ContainerBuildException(MsgCantAddToContainerAfterBuild);
            }
        }
    }

    /// <summary>
    /// Build the <see cref="IDependency" /> object and add it to the dependency list.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="DependencyBuilder" /> object being used to build the
    /// <see cref="IDependency" /> object.
    /// </param>
    private void FinalizeDependency(DependencyBuilder builder)
    {
        CheckForContainerAlreadyBuilt();
        IDependency dependency = builder.Build();
        DependencyList.Add(dependency);
    }
}