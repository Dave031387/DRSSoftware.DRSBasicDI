namespace DRSSoftware.DRSBasicDI;

public class ContainerTests
{
    [Fact]
    public void CreateContainer_ShouldCreateContainer()
    {
        // Arrange
        string containerKey = "Test01";
        MockServiceLocator mockServiceLocator = new();
        Mock<IResolvingObjectsService> mockResolvingObjectService = mockServiceLocator.GetMockNonScopedResolvingObjectsService();
        Mock<IDependencyResolver> mockDependencyResolver = mockServiceLocator.GetMockNonScopedDependencyResolver();
        mockResolvingObjectService
            .Setup(m => m.Add(It.IsAny<IContainer>(), EmptyKey))
            .Returns(It.IsAny<IContainer>())
            .Verifiable(Times.Once);

        // Act
        Container container = new(mockServiceLocator, containerKey);

        // Assert
        container.ContainerKey
            .Should()
            .Be(containerKey);
        mockServiceLocator
            .VerifyAll();
    }

    [Fact]
    public void CreateScope_ShouldReturnScope()
    {
        // Arrange
        string containerKey = "Test02";
        MockServiceLocator mockServiceLocator = new();
        Mock<IResolvingObjectsService> mockResolvingObjectService = mockServiceLocator.GetMockNonScopedResolvingObjectsService();
        Mock<IDependencyResolver> mockDependencyResolver = mockServiceLocator.GetMockNonScopedDependencyResolver();
        Mock<IScope> mockScope = mockServiceLocator.GetMockScope();
        mockResolvingObjectService
            .Setup(m => m.Add(It.IsAny<IContainer>(), EmptyKey))
            .Returns(It.IsAny<IContainer>())
            .Verifiable(Times.Once);
        Container container = new(mockServiceLocator, containerKey);

        // Act
        IScope actual = container.CreateScope();

        // Assert
        actual
            .Should()
            .BeSameAs(mockScope.Object);
        mockServiceLocator
            .VerifyAll();
    }

    [Fact]
    public void CreateTwoContainersWithDifferentKeys_ShouldCreateContainers()
    {
        // Arrange
        string containerKey1 = "Test03";
        string containerKey2 = "Test04";
        MockServiceLocator mockServiceLocator = new();
        Mock<IResolvingObjectsService> mockResolvingObjectService = mockServiceLocator.GetMockNonScopedResolvingObjectsService();
        Mock<IDependencyResolver> mockDependencyResolver = mockServiceLocator.GetMockNonScopedDependencyResolver();
        mockResolvingObjectService
            .Setup(m => m.Add(It.IsAny<IContainer>(), EmptyKey))
            .Returns(It.IsAny<IContainer>())
            .Verifiable(Times.Exactly(2));

        // Act
        Container container1 = new(mockServiceLocator, containerKey1);
        Container container2 = new(mockServiceLocator, containerKey2);

        // Assert
        container1.ContainerKey
            .Should()
            .Be(containerKey1);
        container2.ContainerKey
            .Should()
            .Be(containerKey2);
        mockServiceLocator
            .VerifyAll();
    }

    [Fact]
    public void CreateTwoContainersWithSameKey_ShouldThrowException()
    {
        // Arrange
        string containerKey = "Test05";
        MockServiceLocator mockServiceLocator = new();
        Mock<IResolvingObjectsService> mockResolvingObjectService = mockServiceLocator.GetMockNonScopedResolvingObjectsService();
        Mock<IDependencyResolver> mockDependencyResolver = mockServiceLocator.GetMockNonScopedDependencyResolver();
        mockResolvingObjectService
            .Setup(m => m.Add(It.IsAny<IContainer>(), EmptyKey))
            .Returns(It.IsAny<IContainer>())
            .Verifiable(Times.Once);
        Container container = new(mockServiceLocator, containerKey);
        string expected = MsgContainerAlreadyBuilt;

        // Act
        void action() => _ = new Container(mockServiceLocator, containerKey);

        // Assert
        AssertException<DependencyInjectionException>(action, expected);
        mockServiceLocator
            .VerifyAll();
    }

    [Fact]
    public void GetInstanceWithExistingKey_ShouldReturnContainer()
    {
        // Arrange
        string containerKey1 = "Test06";
        string containerKey2 = "Test07";
        MockServiceLocator mockServiceLocator = new();
        Mock<IResolvingObjectsService> mockResolvingObjectService = mockServiceLocator.GetMockNonScopedResolvingObjectsService();
        Mock<IDependencyResolver> mockDependencyResolver = mockServiceLocator.GetMockNonScopedDependencyResolver();
        mockResolvingObjectService
            .Setup(m => m.Add(It.IsAny<IContainer>(), EmptyKey))
            .Returns(It.IsAny<IContainer>())
            .Verifiable(Times.Exactly(2));
        Container expected1 = new(mockServiceLocator, containerKey1);
        Container expected2 = new(mockServiceLocator, containerKey2);

        // Act
        IContainer actual1 = Container.GetInstance(containerKey1);
        IContainer actual2 = Container.GetInstance(containerKey2);

        // Assert
        actual1
            .Should()
            .BeSameAs(expected1);
        actual2
            .Should()
            .BeSameAs(expected2);
        mockServiceLocator
            .VerifyAll();
    }

    [Fact]
    public void GetInstanceWithNonExistingKey_ShouldThrowException()
    {
        // Arrange
        string expected = MsgContainerNotBuilt;

        // Act
        static void action() => _ = Container.GetInstance("Test08");

        // Assert
        AssertException<DependencyInjectionException>(action, expected);
    }

    [Fact]
    public void GetInstanceWithNullKey_ShouldThrowException()
    {
        // Arrange
        string expected = MsgNullContainerKey;

        // Act
        static void action() => _ = Container.GetInstance(null!);

        // Assert
        AssertException<DependencyInjectionException>(action, expected);
    }

    [Fact]
    public void ResolveDependencyWithoutParameters_ShouldReturnResolvedDependency()
    {
        // Arrange
        string containerKey = "Test09";
        Class1 expected = new();
        MockServiceLocator mockServiceLocator = new();
        Mock<IDependencyResolver> mockDependencyResolver = mockServiceLocator.GetMockNonScopedDependencyResolver();
        mockDependencyResolver
            .Setup(m => m.Resolve<IClass1>(EmptyKey))
            .Returns(expected)
            .Verifiable(Times.Once);
        Mock<IResolvingObjectsService> mockResolvingObjectService = mockServiceLocator.GetMockNonScopedResolvingObjectsService();
        mockResolvingObjectService
            .Setup(m => m.Add(It.IsAny<IContainer>(), EmptyKey))
            .Returns(It.IsAny<IContainer>())
            .Verifiable(Times.Once);
        Container container = new(mockServiceLocator, containerKey);

        // Act
        IClass1 actual = container.Resolve<IClass1>();

        // Assert
        actual
            .Should()
            .BeSameAs(expected);
        mockServiceLocator
            .VerifyAll();
    }

    [Fact]
    public void ResolveDependencyWithParameters_ShouldReturnResolvedDependency()
    {
        // Arrange
        string containerKey = "Test10";
        object[] parameters = [42, "Test"];
        Class1 expected = new();
        MockServiceLocator mockServiceLocator = new();
        Mock<IDependencyResolver> mockDependencyResolver = mockServiceLocator.GetMockNonScopedDependencyResolver();
        mockDependencyResolver
            .Setup(m => m.Resolve<IClass1>(parameters, EmptyKey))
            .Returns(expected)
            .Verifiable(Times.Once);
        Mock<IResolvingObjectsService> mockResolvingObjectService = mockServiceLocator.GetMockNonScopedResolvingObjectsService();
        mockResolvingObjectService
            .Setup(m => m.Add(It.IsAny<IContainer>(), EmptyKey))
            .Returns(It.IsAny<IContainer>())
            .Verifiable(Times.Once);
        Container container = new(mockServiceLocator, containerKey);

        // Act
        IClass1 actual = container.Resolve<IClass1>(parameters);

        // Assert
        actual
            .Should()
            .BeSameAs(expected);
        mockServiceLocator
            .VerifyAll();
    }
}