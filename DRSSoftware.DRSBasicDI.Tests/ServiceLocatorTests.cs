namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

public sealed class ServiceLocatorTests
{
    private const string ContainerKey1 = "Test1";
    private const string ContainerKey2 = "Test2";
    private static readonly IServiceLocator _serviceLocator1 = ServiceLocator.GetInstance(ContainerKey1);
    private static readonly IServiceLocator _serviceLocator2 = ServiceLocator.GetInstance(ContainerKey2);

    [Fact]
    public void GetContainer_ShouldReturnSingletonInstance()
    {
        // Arrange/Act
        IContainer container1 = _serviceLocator1.Get<IContainer>();
        IContainer container2 = _serviceLocator1.Get<IContainer>();

        // Assert
        container1
            .Should()
            .NotBeNull();
        container1
            .Should()
            .BeOfType<Container>();
        container1
            .Should()
            .BeSameAs(container2);
    }

    [Fact]
    public void GetDependencyListBuilder_ShouldReturnSingletonInstance()
    {
        // Arrange/Act
        IDependencyListBuilder dependencyList1 = _serviceLocator1.Get<IDependencyListBuilder>();
        IDependencyListBuilder dependencyList2 = _serviceLocator1.Get<IDependencyListBuilder>();

        // Assert
        dependencyList1
            .Should()
            .NotBeNull();
        dependencyList1
            .Should()
            .BeOfType<DependencyList>();
        dependencyList1
            .Should()
            .BeSameAs(dependencyList2);
    }

    [Fact]
    public void GetDependencyListBuilderAndDependencyListConsumer_ShouldReturnSameInstance()
    {
        // Arrange/Act
        IDependencyListBuilder dependencyList1 = _serviceLocator1.Get<IDependencyListBuilder>();
        IDependencyListConsumer dependencyList2 = _serviceLocator1.Get<IDependencyListConsumer>();

        // Assert
        dependencyList1
            .Should()
            .BeSameAs(dependencyList2);
    }

    [Fact]
    public void GetDependencyListConsumer_ShouldReturnSingletonInstance()
    {
        // Arrange/Act
        IDependencyListConsumer dependencyList1 = _serviceLocator1.Get<IDependencyListConsumer>();
        IDependencyListConsumer dependencyList2 = _serviceLocator1.Get<IDependencyListConsumer>();

        // Assert
        dependencyList1
            .Should()
            .NotBeNull();
        dependencyList1
            .Should()
            .BeOfType<DependencyList>();
        dependencyList1
            .Should()
            .BeSameAs(dependencyList2);
    }

    [Fact]
    public void GetNonScopedDependencyResolver_ShouldNotBeSameAsScopedDependencyResolver()
    {
        // Arrange/Act
        IDependencyResolver dependencyResolver1 = _serviceLocator1.Get<IDependencyResolver>(NonScoped);
        IDependencyResolver dependencyResolver2 = _serviceLocator1.Get<IDependencyResolver>(Scoped);

        // Assert
        dependencyResolver1
            .Should()
            .NotBeSameAs(dependencyResolver2);
    }

    [Fact]
    public void GetNonScopedDependencyResolver_ShouldReturnSingletonInstance()
    {
        // Arrange/Act
        IDependencyResolver dependencyResolver1 = _serviceLocator1.Get<IDependencyResolver>(NonScoped);
        IDependencyResolver dependencyResolver2 = _serviceLocator1.Get<IDependencyResolver>(NonScoped);

        // Assert
        dependencyResolver1
            .Should()
            .NotBeNull();
        dependencyResolver1
            .Should()
            .BeOfType<DependencyResolver>();
        dependencyResolver1
            .Should()
            .BeSameAs(dependencyResolver2);
    }

    [Fact]
    public void GetNonScopedResolvingObjectService_ShouldNotBeSameAsScopedResolvingObjectService()
    {
        // Arrange/Act
        IResolvingObjectsService resolvingObjectsService1 = _serviceLocator1.Get<IResolvingObjectsService>(NonScoped);
        IResolvingObjectsService resolvingObjectsService2 = _serviceLocator1.Get<IResolvingObjectsService>(Scoped);

        // Assert
        resolvingObjectsService1
            .Should()
            .NotBeSameAs(resolvingObjectsService2);
    }

    [Fact]
    public void GetNonScopedResolvingObjectService_ShouldReturnSingletonInstance()
    {
        // Arrange/Act
        IResolvingObjectsService resolvingObjectsService1 = _serviceLocator1.Get<IResolvingObjectsService>(NonScoped);
        IResolvingObjectsService resolvingObjectsService2 = _serviceLocator1.Get<IResolvingObjectsService>(NonScoped);

        // Assert
        resolvingObjectsService1
            .Should()
            .NotBeNull();
        resolvingObjectsService1
            .Should()
            .BeOfType<ResolvingObjectsService>();
        resolvingObjectsService1
            .Should()
            .BeSameAs(resolvingObjectsService2);
    }

    [Fact]
    public void GetObjectConstructor_ShouldReturnSingletonInstance()
    {
        // Arrange/Act
        IObjectConstructor objectConstructor1 = _serviceLocator1.Get<IObjectConstructor>();
        IObjectConstructor objectConstructor2 = _serviceLocator1.Get<IObjectConstructor>();

        // Assert
        objectConstructor1
            .Should()
            .NotBeNull();
        objectConstructor1
            .Should()
            .BeOfType<ObjectConstructor>();
        objectConstructor1
            .Should()
            .BeSameAs(objectConstructor2);
    }

    [Fact]
    public void GetScope_ShouldReturnNewInstanceEachTime()
    {
        // Arrange/Act
        IScope scope1 = _serviceLocator1.Get<IScope>();
        IScope scope2 = _serviceLocator1.Get<IScope>();

        // Assert
        scope1
            .Should()
            .NotBeNull();
        scope1
            .Should()
            .BeOfType<Scope>();
        scope2
            .Should()
            .NotBeNull();
        scope2
            .Should()
            .BeOfType<Scope>();
        scope1
            .Should()
            .NotBeSameAs(scope2);
    }

    [Fact]
    public void GetScopedDependencyResolver_ShouldReturnNewInstanceEachTime()
    {
        // Arrange/Act
        IDependencyResolver dependencyResolver1 = _serviceLocator1.Get<IDependencyResolver>(Scoped);
        IDependencyResolver dependencyResolver2 = _serviceLocator1.Get<IDependencyResolver>(Scoped);

        // Assert
        dependencyResolver1
            .Should()
            .NotBeNull();
        dependencyResolver1
            .Should()
            .BeOfType<DependencyResolver>();
        dependencyResolver2
            .Should()
            .NotBeNull();
        dependencyResolver2
            .Should()
            .BeOfType<DependencyResolver>();
        dependencyResolver1
            .Should()
            .NotBeSameAs(dependencyResolver2);
    }

    [Fact]
    public void GetScopedResolvingObjectService_ShouldReturnNewInstanceEachTime()
    {
        // Arrange/Act
        IResolvingObjectsService resolvingObjectsService1 = _serviceLocator1.Get<IResolvingObjectsService>(Scoped);
        IResolvingObjectsService resolvingObjectsService2 = _serviceLocator1.Get<IResolvingObjectsService>(Scoped);

        // Assert
        resolvingObjectsService1
            .Should()
            .NotBeNull();
        resolvingObjectsService1
            .Should()
            .BeOfType<ResolvingObjectsService>();
        resolvingObjectsService2
            .Should()
            .NotBeNull();
        resolvingObjectsService2
            .Should()
            .BeOfType<ResolvingObjectsService>();
        resolvingObjectsService1
            .Should()
            .NotBeSameAs(resolvingObjectsService2);
    }

    [Fact]
    public void GetServiceLocatorUsingDifferentContainerKey_ShouldReturnNewServiceLocatorInstance()
    {
        // Assert
        _serviceLocator1
            .Should()
            .NotBeNull();
        _serviceLocator2
            .Should()
            .NotBeNull();
        _serviceLocator2
            .Should()
            .NotBeSameAs(_serviceLocator1);
    }

    [Fact]
    public void GetServiceLocatorUsingStaticMethod_ShouldReturnSingletonInstanceOfServiceLocator()
    {
        // Arrange/Act
        IServiceLocator serviceLocator = ServiceLocator.GetInstance(ContainerKey1);

        // Assert
        serviceLocator
            .Should()
            .BeSameAs(_serviceLocator1);
    }

    [Fact]
    public void GetSingletonInstanceFromTwoDifferentServiceLocators_ShouldReturnDifferentInstances()
    {
        // Arrange/Act
        IContainer container1 = _serviceLocator1.Get<IContainer>();
        IContainer container2 = _serviceLocator2.Get<IContainer>();

        // Assert
        container1
            .Should()
            .NotBeNull();
        container2
            .Should()
            .NotBeNull();
        container1
            .Should()
            .NotBeSameAs(container2);
    }

    [Fact]
    public void TryToGetUnregisteredService_ShouldThrowException()
    {
        // Arrange
        string expected = FormatMessage<IClass1>(MsgServiceNotRegistered);

        // Act
        static void action() => _serviceLocator1.Get<IClass1>();

        // Assert
        AssertException<ServiceLocatorException>(action, expected);
    }
}