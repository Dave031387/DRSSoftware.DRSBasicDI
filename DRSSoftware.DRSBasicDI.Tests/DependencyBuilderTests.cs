namespace DRSSoftware.DRSBasicDI;

public class DependencyBuilderTests
{
    [Fact]
    public void BuildDependencyWithAbstractResolvingClassType_ShouldThrowException()
    {
        // Arrange
        Type resolvingType = typeof(AbstractClass1);
        string expected = FormatMessage(MsgAbstractResolvingType, typeof(IClass1), EmptyKey, resolvingType);

        // Act
        void action() => _ = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithLifetime(DependencyLifetime.Scoped)
            .WithResolvingType(resolvingType)
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildDependencyWithDependencyTypeAndResolvingTypeAreBothClassTypes_ShouldReturnDependency()
    {
        // Arrange
        IDependency expected = CreateTransientDependency<Class1, Class1A>();

        // Act
        IDependency actual = DependencyBuilder
            .CreateNew
            .WithDependencyType<Class1>()
            .WithResolvingType<Class1A>()
            .WithLifetime(DependencyLifetime.Transient)
            .Build();

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void BuildDependencyWithInvalidDependencyType_ShouldThrowException()
    {
        // Arrange
        string expected = FormatMessage(MsgInvalidDependencyType, typeof(int), EmptyKey);

        // Act
        static void action() => _ = DependencyBuilder
            .CreateNew
            .WithResolvingType<Class1>()
            .WithLifetime(DependencyLifetime.Singleton)
            .WithDependencyType(typeof(int))
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildDependencyWithMissingDependencyType_ShouldThrowException()
    {
        // Arrange/Act
        static void action() => _ = DependencyBuilder
            .CreateNew
            .WithResolvingType<Class1>()
            .WithLifetime(DependencyLifetime.Singleton)
            .WithResolvingKey("test")
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, MsgUnspecifiedDependencyType);
    }

    [Fact]
    public void BuildDependencyWithMissingLifetime_ShouldThrowException()
    {
        // Arrange
        string expected = FormatMessage(MsgUndefinedLifetime, typeof(IClass1), EmptyKey);

        // Act
        static void action() => _ = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithResolvingType<Class1>()
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildDependencyWithMissingResolvingType_ShouldThrowException()
    {
        // Arrange
        string expected = FormatMessage(MsgUnspecifiedResolvingType, typeof(IClass1), EmptyKey);

        // Act
        static void action() => _ = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithLifetime(DependencyLifetime.Scoped)
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildDependencyWithMoreThanOneDependencyType_ShouldThrowException()
    {
        // Arrange
        string expected = FormatMessage(MsgDependencyTypeAlreadySpecified, typeof(IClass1), EmptyKey);

        // Act
        static void action() => _ = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithResolvingType<Class1>()
            .WithLifetime(DependencyLifetime.Singleton)
            .WithDependencyType<IClass2>()
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildDependencyWithMoreThanOneLifetime_ShouldThrowException()
    {
        // Arrange
        string expected = FormatMessage(MsgLifetimeAlreadySpecified, typeof(IClass1), EmptyKey);

        // Act
        static void action() => _ = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithLifetime(DependencyLifetime.Singleton)
            .WithResolvingType<Class1>()
            .WithLifetime(DependencyLifetime.Transient)
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildDependencyWithMoreThanOneResolvingKey_ShouldThrowException()
    {
        // Arrange
        string resolvingKey = "test1";
        string expected = FormatMessage(MsgResolvingKeyAlreadySpecified, typeof(IClass1), resolvingKey);

        // Act
        void action() => _ = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithResolvingKey(resolvingKey)
            .WithLifetime(DependencyLifetime.Singleton)
            .WithResolvingType<Class1>()
            .WithResolvingKey("test2")
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildDependencyWithMoreThanOneResolvingType_ShouldThrowException()
    {
        // Arrange
        string expected = FormatMessage(MsgResolvingTypeAlreadySpecified, typeof(IClass1), EmptyKey);

        // Act
        static void action() => _ = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithResolvingType<Class1>()
            .WithLifetime(DependencyLifetime.Singleton)
            .WithResolvingType<Class2>()
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildDependencyWithNullDependencyType_ShouldThrowException()
    {
        // Arrange/Act
        static void action() => _ = DependencyBuilder
            .CreateNew
            .WithResolvingType<Class1>()
            .WithLifetime(DependencyLifetime.Singleton)
            .WithDependencyType(null!)
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, MsgNullDependencyType);
    }

    [Fact]
    public void BuildDependencyWithNullResolvingKey_ShouldThrowException()
    {
        // Arrange
        string expected = FormatMessage(MsgNullResolvingKey, typeof(IClass1), EmptyKey);

        // Act
        static void action() => _ = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithResolvingType<Class1>()
            .WithLifetime(DependencyLifetime.Singleton)
            .WithResolvingKey(null!)
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildDependencyWithNullResolvingType_ShouldThrowException()
    {
        // Arrange
        string expected = FormatMessage(MsgNullResolvingType, typeof(IClass1), EmptyKey);

        // Act
        static void action() => _ = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithLifetime(DependencyLifetime.Singleton)
            .WithResolvingType(null!)
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildDependencyWithOpenGenericDependencyType_ShouldThrowException()
    {
        // Arrange
        Type dependencyType = typeof(IGenericClass1<int, int>).GetGenericTypeDefinition();
        string resolvingKey = "test";
        string expected = FormatMessage(MsgGenericDependencyTypeIsOpen, dependencyType, resolvingKey);

        // Act
        void action() => _ = DependencyBuilder
            .CreateNew
            .WithResolvingType<Class1>()
            .WithLifetime(DependencyLifetime.Singleton)
            .WithResolvingKey(resolvingKey)
            .WithDependencyType(dependencyType)
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildDependencyWithOpenGenericResolvingType_ShouldThrowException()
    {
        // Arrange
        Type resolvingType = typeof(GenericClass1<int, int>).GetGenericTypeDefinition();
        string expected = FormatMessage(MsgResolvingGenericTypeIsOpen, typeof(IClass1), EmptyKey, resolvingType);

        // Act
        void action() => _ = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithLifetime(DependencyLifetime.Scoped)
            .WithResolvingType(resolvingType)
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildDependencyWithResolvingTypeNotAClassType_ShouldThrowException()
    {
        // Arrange
        Type resolvingType = typeof(int);
        string expected = FormatMessage(MsgInvalidResolvingType, typeof(IClass1), EmptyKey, resolvingType);

        // Act
        void action() => _ = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithLifetime(DependencyLifetime.Scoped)
            .WithResolvingType(resolvingType)
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildDependencyWithResolvingTypeNotAssignableToDependencyType_ShouldThrowException()
    {
        // Arrange
        string expected = FormatMessage(MsgIncompatibleResolvingType, typeof(IClass1), EmptyKey, typeof(Class2));

        // Act
        static void action() => _ = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithLifetime(DependencyLifetime.Scoped)
            .WithResolvingType<Class2>()
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildDependencyWithUndefinedLifetime_ShouldThrowException()
    {
        // Arrange
        string expected = FormatMessage(MsgUndefinedLifetime, typeof(IClass1), EmptyKey);

        // Act
        static void action() => _ = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithResolvingType<Class1>()
            .WithLifetime(DependencyLifetime.Undefined)
            .Build();

        // Assert
        AssertException<DependencyBuildException>(action, expected);
    }

    [Fact]
    public void BuildEmptyDependency_ShouldThrowException()
    {
        // Arrange/Act
        static void action() => _ = DependencyBuilder.CreateNew.Build();

        // Assert
        AssertException<DependencyBuildException>(action, MsgUnspecifiedDependencyType);
    }

    [Fact]
    public void BuildScopedDependencyWithResolvingKey_ShouldReturnDependency()
    {
        // Arrange
        string resolvingKey = "test";
        IDependency expected = CreateScopedDependency<IClass1, Class1>(resolvingKey);

        // Act
        IDependency actual = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithResolvingType<Class1>()
            .WithLifetime(DependencyLifetime.Scoped)
            .WithResolvingKey(resolvingKey)
            .Build();

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void BuildSingletonDependencyUsingGenericDependencyTypeAndResolvingType_ShouldReturnDependency()
    {
        // Arrange
        IDependency expected = CreateSingletonDependency<IClass1, Class1>();

        // Act
        IDependency actual = DependencyBuilder
            .CreateNew
            .WithDependencyType<IClass1>()
            .WithResolvingType<Class1>()
            .WithLifetime(DependencyLifetime.Singleton)
            .Build();

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void BuildTransientDependencyUsingNongenericDependencyTypeAndResolvingType_ShouldReturnDependency()
    {
        // Arrange
        Type dependencyType = typeof(IClass1);
        Type resolvingType = typeof(Class1);
        IDependency expected = CreateTransientDependency<IClass1, Class1>();

        // Act
        IDependency actual = DependencyBuilder
            .CreateNew
            .WithDependencyType(dependencyType)
            .WithResolvingType(resolvingType)
            .WithLifetime(DependencyLifetime.Transient)
            .Build();

        // Assert
        actual
            .Should()
            .Be(expected);
    }
}