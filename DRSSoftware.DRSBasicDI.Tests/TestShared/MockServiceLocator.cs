namespace DRSSoftware.DRSBasicDI.TestShared;

using DRSSoftware.DRSBasicDI.Interfaces;

internal sealed class MockServiceLocator : IServiceLocator
{
    private readonly object _lock = new();

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
            ValidateMockObjectRequest(Container, "Container", resolvingKey);
            return (T)Container!.Object;
        }

        if (type == typeof(IDependencyListBuilder) || type == typeof(IDependencyListConsumer))
        {
            ValidateMockObjectRequest(DependencyList, "DependencyList", resolvingKey);
            return (T)DependencyList!.Object;
        }

        if (type == typeof(IDependencyResolver))
        {
            ValidateMockObjectRequest(NonScopedDependencyResolver, ScopedDependencyResolver, "DependencyResolver", resolvingKey);

            if (resolvingKey is NonScoped)
            {
                return (T)NonScopedDependencyResolver!.Object;
            }
            else
            {
                return (T)ScopedDependencyResolver!.Object;
            }
        }

        if (type == typeof(IObjectConstructor))
        {
            ValidateMockObjectRequest(ObjectConstructor, "ObjectConstructor", resolvingKey);
            return (T)ObjectConstructor!.Object;
        }

        if (type == typeof(IResolvingObjectsService))
        {
            ValidateMockObjectRequest(NonScopedResolvingObjectsService, ScopedResolvingObjectsService, "ResolvingObjectService", resolvingKey);

            if (resolvingKey is NonScoped)
            {
                return (T)NonScopedResolvingObjectsService!.Object;
            }
            else
            {
                return (T)ScopedResolvingObjectsService!.Object;
            }
        }

        if (type == typeof(IScope))
        {
            ValidateMockObjectRequest(Scope, "Scope", resolvingKey);
            return (T)Scope!.Object;
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

    public void VerifyAll()
    {
        Container?.VerifyAll();
        DependencyList?.VerifyAll();
        NonScopedDependencyResolver?.VerifyAll();
        NonScopedResolvingObjectsService?.VerifyAll();
        ObjectConstructor?.VerifyAll();
        Scope?.VerifyAll();
        ScopedDependencyResolver?.VerifyAll();
        ScopedResolvingObjectsService?.VerifyAll();
    }

    private static void ValidateMockObjectRequest(object? mockObject, string objectName, string resolvingKey)
    {
        mockObject
            .Should()
            .NotBeNull($"The mock {objectName} must be created before it is used.");

        resolvingKey
            .Should()
            .Be(EmptyKey, $"The resolving key must be empty for the mock {objectName} object.");
    }

    private static void ValidateMockObjectRequest(object? mockNonScopedObject, object? mockScopedObject, string objectName, string resolvingKey)
    {
        resolvingKey
            .Should()
            .BeOneOf([NonScoped, Scoped], $"The resolving key must be either NonScoped or Scoped for the mock {objectName} object.");

        if (resolvingKey is NonScoped)
        {
            mockNonScopedObject
                .Should()
                .NotBeNull($"The mock NonScoped {objectName} must be created before it is used.");
        }
        else
        {
            mockScopedObject
                .Should()
                .NotBeNull($"The mock Scoped {objectName} must be created before it is used.");
        }
    }
}