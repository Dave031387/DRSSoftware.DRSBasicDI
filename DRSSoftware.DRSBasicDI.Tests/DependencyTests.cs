namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

public class DependencyTests
{
    [Fact]
    public void CompareDependencyToNull_ShouldReturnFalse()
    {
        // Arrange
        IDependency? dependency1 = CreateSingletonDependency<IClass1, Class1>();
        IDependency? dependency2 = null;

        // Act
        bool actual = dependency1.Equals(dependency2);

        // Assert
        actual
            .Should()
            .BeFalse();
    }

    [Fact]
    public void CompareTwoEqualDependencies_ShouldReturnTrue()
    {
        // Arrange
        IDependency dependency1 = CreateScopedDependency<IClass1, Class1>("test");
        IDependency dependency2 = CreateScopedDependency<IClass1, Class1>("test");

        // Act
        bool actual = dependency1.Equals(dependency2);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Fact]
    public void CompareTwoUnequalDependencies_ShouldReturnFalse()
    {
        // Arrange
        IDependency dependency1 = CreateScopedDependency<IClass1, Class1>();
        IDependency dependency2 = CreateScopedDependency<IClass1, Class1>("test");

        // Act
        bool actual = dependency1.Equals(dependency2);

        // Assert
        actual
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ConstructDependency_AllPropertiesShouldBeSetCorrectly()
    {
        // Arrange
        string resolvingKey = "test";
        ServiceKey expectedDependencyServiceKey = new(typeof(IClass1), resolvingKey);
        ServiceKey expectedResolvingServiceKey = new(typeof(Class1), resolvingKey);

        // Act
        IDependency dependency = CreateTransientDependency<IClass1, Class1>(resolvingKey);

        // Assert
        dependency
            .Should()
            .NotBeNull()
            .And
            .BeOfType<Dependency>();
        dependency.DependencyType
            .Should()
            .Be<IClass1>();
        dependency.ResolvingType
            .Should()
            .Be<Class1>();
        dependency.Lifetime
            .Should()
            .Be(DependencyLifetime.Transient);
        dependency.ResolvingKey
            .Should()
            .Be(resolvingKey);
        dependency.DependencyServiceKey
            .Should()
            .Be(expectedDependencyServiceKey);
        dependency.ResolvingServiceKey
            .Should()
            .Be(expectedResolvingServiceKey);
    }

    [Fact]
    public void GetHashCodeForTwoDifferentDependencies_ShouldReturnTwoDifferentValues()
    {
        // Arrange
        IDependency dependency1 = CreateTransientDependency<IClass1, Class1>("key1");
        IDependency dependency2 = CreateTransientDependency<IClass1, Class1>("key2");

        // Act
        int hashCode1 = dependency1.GetHashCode();
        int hashCode2 = dependency2.GetHashCode();

        // Assert
        hashCode1
            .Should()
            .NotBe(hashCode2);
    }

    [Fact]
    public void GetHashCodeForTwoSimilarDependencies_ShouldReturnSameValue()
    {
        // Arrange (only the dependency type and resolving key must match)
        IDependency dependency1 = CreateTransientDependency<IClass1, Class1>("key");
        IDependency dependency2 = CreateSingletonDependency<IClass1, Class2>("key");

        // Act
        int hashCode1 = dependency1.GetHashCode();
        int hashCode2 = dependency2.GetHashCode();

        // Assert
        hashCode1
            .Should()
            .Be(hashCode2);
    }
}