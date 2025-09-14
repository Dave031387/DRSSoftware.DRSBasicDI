namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

public class DependencyListTests
{
    [Fact]
    public void CreateDependencyListObject_ShouldAddContainerDependency()
    {
        // Arrange
        IDependency dependency = CreateSingletonDependency<IContainer, Container>();
        ServiceKey serviceKey = new(typeof(IContainer), EmptyKey);
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
}
