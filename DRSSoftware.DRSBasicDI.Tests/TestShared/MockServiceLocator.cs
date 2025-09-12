namespace DRSSoftware.DRSBasicDI.TestShared;

using DRSSoftware.DRSBasicDI.Interfaces;

internal sealed class MockServiceLocator : IServiceLocator
{
    private readonly object _lock = new();
    private readonly List<string> _validResolvingKeys = [NonScoped, Scoped];

    private Mock<IContainer>? Container
    {
        get; set;
    }

    private Mock<IDependencyList>? DependencyList
    {
        get; set;
    }

    private Mock<IDependencyResolver>? NonScopedDependencyResolver
    {
        get; set;
    }

    private Mock<IResolvingObjectsService>? NonScopedResolvingObjectsService
    {
        get; set;
    }

    private Mock<IObjectConstructor>? ObjectConstructor
    {
        get; set;
    }

    private Mock<IScope>? Scope
    {
        get; set;
    }

    private Mock<IDependencyResolver>? ScopedDependencyResolver
    {
        get; set;
    }

    private Mock<IResolvingObjectsService>? ScopedResolvingObjectsService
    {
        get; set;
    }

    public T Get<T>(string resolvingKey = EmptyKey) where T : class
    {
        Type type = typeof(T);

        if (type == typeof(IContainer))
        {
            Container
                .Should()
                .NotBeNull("The mock Container must be created before it is used.");
            resolvingKey
                .Should()
                .Be(EmptyKey, "The resolving key must be empty for the mock Container object.");
            return (T)Container.Object;
        }

        if (type == typeof(IDependencyListBuilder) || type == typeof(IDependencyListConsumer))
        {
            DependencyList
                .Should()
                .NotBeNull("The mock DependencyList must be created before it is used.");
            resolvingKey
                .Should()
                .Be(EmptyKey, "The resolving key must be empty for the mock DependencyList object.");
            return (T)DependencyList.Object;
        }

        if (type == typeof(IDependencyResolver))
        {
            resolvingKey
                .Should()
                .BeOneOf(_validResolvingKeys, "The resolving key must be either NonScoped or Scoped for the mock DependencyResolver object.");

            if (resolvingKey is NonScoped)
            {
                NonScopedDependencyResolver
                    .Should()
                    .NotBeNull("The mock NonScoped DependencyResolver must be created before it is used.");
                return (T)NonScopedDependencyResolver.Object;
            }
            else
            {
                ScopedDependencyResolver
                    .Should()
                    .NotBeNull("The mock Scoped DependencyResolver must be created before it is used.");
                return (T)ScopedDependencyResolver.Object;
            }
        }

        if (type == typeof(IObjectConstructor))
        {
            ObjectConstructor
                .Should()
                .NotBeNull("The mock ObjectConstructor must be created before it is used.");
            resolvingKey
                .Should()
                .Be(EmptyKey, "The resolving key must be empty for the mock ObjectConstructor object.");
            return (T)ObjectConstructor.Object;
        }

        if (type == typeof(IResolvingObjectsService))
        {
            resolvingKey
                .Should()
                .BeOneOf(_validResolvingKeys, "The resolving key must be either NonScoped or Scoped for the mock ResolvingObjectsService object.");

            if (resolvingKey is NonScoped)
            {
                NonScopedResolvingObjectsService
                    .Should()
                    .NotBeNull("The mock NonScoped ResolvingObjectsService must be created before it is used.");
                return (T)NonScopedResolvingObjectsService.Object;
            }
            else
            {
                ScopedResolvingObjectsService
                    .Should()
                    .NotBeNull("The mock Scoped ResolvingObjectsService must be created before it is used.");
                return (T)ScopedResolvingObjectsService.Object;
            }
        }

        if (type == typeof(IScope))
        {
            Scope
                .Should()
                .NotBeNull("The mock Scope must be created before it is used.");
            resolvingKey
                .Should()
                .Be(EmptyKey, "The resolving key must be empty for the mock Scope object.");
            return (T)Scope.Object;
        }

        throw new TypeAccessException($"The type {type.FullName} is not known to the mock service locator.");
    }

    public Mock<IContainer> GetMockContainer()
    {
        if (Container is null)
        {
            lock (_lock)
            {
                Container ??= new Mock<IContainer>(MockBehavior.Strict);
            }
        }

        return Container;
    }

    public Mock<IDependencyList> GetMockDependencyList()
    {
        if (DependencyList is null)
        {
            lock (_lock)
            {
                DependencyList ??= new Mock<IDependencyList>(MockBehavior.Strict);
            }
        }

        return DependencyList;
    }

    public Mock<IDependencyResolver> GetMockNonScopedDependencyResolver()
    {
        if (NonScopedDependencyResolver is null)
        {
            lock (_lock)
            {
                NonScopedDependencyResolver ??= new Mock<IDependencyResolver>(MockBehavior.Strict);
            }
        }

        return NonScopedDependencyResolver;
    }

    public Mock<IResolvingObjectsService> GetMockNonScopedResolvingObjectsService()
    {
        if (NonScopedResolvingObjectsService is null)
        {
            lock (_lock)
            {
                NonScopedResolvingObjectsService ??= new Mock<IResolvingObjectsService>(MockBehavior.Strict);
            }
        }

        return NonScopedResolvingObjectsService;
    }

    public Mock<IObjectConstructor> GetMockObjectConstructor()
    {
        if (ObjectConstructor is null)
        {
            lock (_lock)
            {
                ObjectConstructor ??= new Mock<IObjectConstructor>(MockBehavior.Strict);
            }
        }

        return ObjectConstructor;
    }

    public Mock<IScope> GetMockScope()
    {
        if (Scope is null)
        {
            lock (_lock)
            {
                Scope ??= new Mock<IScope>(MockBehavior.Strict);
            }
        }

        return Scope;
    }

    public Mock<IDependencyResolver> GetMockScopedDependencyResolver()
    {
        if (ScopedDependencyResolver is null)
        {
            lock (_lock)
            {
                ScopedDependencyResolver ??= new Mock<IDependencyResolver>(MockBehavior.Strict);
            }
        }

        return ScopedDependencyResolver;
    }

    public Mock<IResolvingObjectsService> GetMockScopedResolvingObjectsService()
    {
        if (ScopedResolvingObjectsService is null)
        {
            lock (_lock)
            {
                ScopedResolvingObjectsService ??= new Mock<IResolvingObjectsService>(MockBehavior.Strict);
            }
        }

        return ScopedResolvingObjectsService;
    }

    public void Reset()
    {
        Container?.Reset();
        DependencyList?.Reset();
        NonScopedDependencyResolver?.Reset();
        NonScopedResolvingObjectsService?.Reset();
        ObjectConstructor?.Reset();
        Scope?.Reset();
        ScopedDependencyResolver?.Reset();
        ScopedResolvingObjectsService?.Reset();
    }
}