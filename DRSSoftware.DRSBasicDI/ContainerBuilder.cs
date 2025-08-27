namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="ContainerBuilder" /> class is used for constructing a new
/// <see cref="Container" /> object to be used for dependency injection.
/// </summary>
/// <remarks>
/// Once the <see cref="Build" /> method has been called, any other method calls other than
/// <see cref="GetTestInstance(IServiceLocater)" /> will result in a
/// <see cref="ContainerBuildException" /> being thrown.
/// </remarks>
public sealed class ContainerBuilder : IContainerBuilder
{
    /// <summary>
    /// A lazy initializer for the singleton instance of the <see cref="ContainerBuilder" /> object.
    /// </summary>
    private static readonly Lazy<ContainerBuilder> _lazy = new(static () => new ContainerBuilder());

    /// <summary>
    /// A boolean flag that gets set to <see langword="true" /> once the <see cref="IContainer" />
    /// object has been built.
    /// </summary>
    private bool _containerHasBeenBuilt;

    /// <summary>
    /// Default constructor for the <see cref="ContainerBuilder" /> class.
    /// </summary>
    /// <remarks>
    /// This constructor is declared <see langword="private" />. Use the static
    /// <see cref="Instance" /> property to create a new, empty <see cref="ContainerBuilder" />
    /// object.
    /// </remarks>
    private ContainerBuilder() : this(DRSBasicDI.ServiceLocater.Instance)
    {
    }

    /// <summary>
    /// Create a new instance of the <see cref="ContainerBuilder" /> class using the specified
    /// <paramref name="serviceLocater" /> object. This constructor is intended for unit testing
    /// only.
    /// </summary>
    /// <param name="serviceLocater">
    /// A service locater object that should provide mock instances of the requested dependencies.
    /// </param>
    /// <remarks>
    /// This constructor is declared <see langword="private" />. Use the static
    /// <see cref="GetTestInstance(IServiceLocater)" /> method to create a new, empty
    /// <see cref="IContainerBuilder" /> object for testing purposes.
    /// </remarks>
    private ContainerBuilder(IServiceLocater serviceLocater)
    {
        ServiceLocater = serviceLocater;
        DependencyList = ServiceLocater.Get<IDependencyListBuilder>();
    }

    /// <summary>
    /// Get the <see cref="IContainerBuilder" /> instance.
    /// </summary>
    /// <remarks>
    /// This returns a thread safe singleton instance of the <see cref="ContainerBuilder" /> class.
    /// </remarks>
    public static IContainerBuilder Instance => _lazy.Value;

    /// <summary>
    /// Get a reference to the <see cref="IDependencyListBuilder" /> object.
    /// </summary>
    private IDependencyListBuilder DependencyList
    {
        get;
    }

    /// <summary>
    /// Get a reference to the <see cref="IServiceLocater" /> object.
    /// </summary>
    private IServiceLocater ServiceLocater
    {
        get;
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
        CheckForContainerAlreadyBuilt();
        IDependency dependency = builder(DependencyBuilder.CreateNew).Build();
        DependencyList.Add(dependency);
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
            string msg = MsgContainerIsEmpty;
            throw new ContainerBuildException(msg);
        }

        IContainer container = ServiceLocater.Get<IContainer>();

        _containerHasBeenBuilt = true;
        return container;
    }

    /// <summary>
    /// Get a new test instance of the <see cref="IContainerBuilder" /> object.
    /// </summary>
    /// <param name="serviceLocater">
    /// A service locater object that should provide mock instances of the requested dependencies.
    /// </param>
    /// <remarks>
    /// Please note that this method doesn't return a singleton instance. Each call to this method
    /// will return a new <see cref="IContainerBuilder" /> object.
    /// </remarks>
    /// <returns>
    /// An instance of the <see cref="IContainerBuilder" /> object to be used for unit testing.
    /// </returns>
    internal static IContainerBuilder GetTestInstance(IServiceLocater serviceLocater)
        => new ContainerBuilder(serviceLocater);

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