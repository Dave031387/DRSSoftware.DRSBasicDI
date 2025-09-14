namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

public class DependencyListTests
{
    [Fact]
    public void AddDuplicateDependency_ShouldThrowException()
    {
        // Arrange
        string resolvingKey = "test";
        IDependency dependency1 = CreateSingletonDependency<IClass1, Class1>(resolvingKey);
        IDependency dependency2 = CreateTransientDependency<IClass1, Class2>(resolvingKey);
        DependencyList dependencyList = new();
        dependencyList.Add(dependency1);
        void action() => dependencyList.Add(dependency2);
        string expected = FormatMessage<IClass1>(MsgDuplicateDependency, resolvingKey);

        // Act/Assert
        AssertException<ContainerBuildException>(action, expected);
    }

    [Fact]
    public void AddNewDependency_ShouldAddDependency()
    {
        // Arrange
        IDependency dependency = CreateSingletonDependency<IClass1, Class1>();
        ServiceKey serviceKey = dependency.DependencyServiceKey;
        int expectedCount = 2;
        DependencyList dependencyList = new();

        // Act
        dependencyList.Add(dependency);

        // Assert
        dependencyList.Count
            .Should()
            .Be(expectedCount);
        dependencyList._dependencies
            .Should()
            .ContainKey(serviceKey);
        dependencyList._dependencies[serviceKey]
            .Should()
            .Be(dependency);
    }

    [Fact]
    public void CreateDependencyListObject_ShouldAddContainerDependency()
    {
        // Arrange
        IDependency dependency = CreateSingletonDependency<IContainer, Container>();
        ServiceKey serviceKey = dependency.DependencyServiceKey;
        int expectedCount = 1;

        // Act
        DependencyList dependencyList = new();

        // Assert
        dependencyList
            .Should()
            .NotBeNull();
        dependencyList.Count
            .Should()
            .Be(expectedCount);
        dependencyList._dependencies
            .Should()
            .ContainKey(serviceKey);
        dependencyList._dependencies[serviceKey]
            .Should()
            .Be(dependency);
    }

    [Fact]
    public void GetNonExistingDependency_ShouldThrowException()
    {
        // Arrange
        string resolvingKey = "test";
        IDependency dependency = CreateSingletonDependency<IClass1, Class1>();
        DependencyList dependencyList = new();
        dependencyList.Add(dependency);
        void action() => dependencyList.Get<IClass1>(resolvingKey);
        string expected = FormatMessage<IClass1>(MsgDependencyMappingNotFound, resolvingKey);

        // Act/Assert
        AssertException<DependencyInjectionException>(action, expected);
    }

    [Fact]
    public void GetNullDependency_ShouldThrowException()
    {
        // Arrange
        ServiceKey serviceKey = new(typeof(IClass1), EmptyKey);
        DependencyList dependencyList = new();
        dependencyList._dependencies[serviceKey] = null!;
        void action() => dependencyList.Get<IClass1>(EmptyKey);
        string expected = FormatMessage<IClass1>(MsgNullDependencyObject);

        // Act/Assert
        AssertException<DependencyInjectionException>(action, expected);
    }

    [Fact]
    public void GetValidDependency_ShouldReturnDependency()
    {
        // Arrange
        IDependency expected = CreateScopedDependency<IClass1, Class1>();
        ServiceKey serviceKey = expected.DependencyServiceKey;
        DependencyList dependencyList = new();
        dependencyList._dependencies[serviceKey] = expected;

        // Act
        IDependency actual = dependencyList.Get<IClass1>(EmptyKey);

        // Assert
        actual
            .Should()
            .NotBeNull()
            .And
            .Be(expected);
    }
}