namespace DRSSoftware.DRSBasicDI;

using System.Reflection;

public sealed class ObjectConstructorTests
{
    [Fact]
    public void ConstructorInfoIsNull_ShouldThrowException()
    {
        // Arrange
        ObjectConstructor objectConstructor = new();
        void action() => objectConstructor.Construct<IClass1>(null!, [], EmptyKey);
        string expected = FormatMessage<IClass1>(MsgErrorDuringConstruction, EmptyKey, typeof(IClass1));

        // Act/Assert
        AssertException<DependencyInjectionException>(action, expected);
    }

    [Fact]
    public void IncorrectParametersPassedIntoConstructor_ShouldThrowException()
    {
        // Arrange
        ObjectConstructor objectConstructor = new();
        ConstructorInfo constructorInfo = typeof(Class1).GetConstructor(BindingFlags.Instance | BindingFlags.Public, [typeof(string)])!;
        int parameter = 42;
        void action() => objectConstructor.Construct<IClass1>(constructorInfo, [parameter], EmptyKey);
        string expected = FormatMessage<IClass1>(MsgErrorDuringConstruction, EmptyKey, typeof(Class1));

        // Act/Assert
        AssertException<DependencyInjectionException>(action, expected);
    }

    [Fact]
    public void ResolvingObjectNotAssignableToDependencyObject_ShouldThrowException()
    {
        // Arrange
        ObjectConstructor objectConstructor = new();
        ConstructorInfo constructorInfo = typeof(Class1).GetConstructor(BindingFlags.Instance | BindingFlags.Public, [])!;
        void action() => objectConstructor.Construct<IClass2>(constructorInfo, [], EmptyKey);
        string innerMessage = FormatMessage<IClass2>(MsgResolvingObjectNotCreated, EmptyKey, typeof(Class1));
        string outerMessage = FormatMessage<IClass2>(MsgErrorDuringConstruction, EmptyKey, typeof(Class1));

        // Act/Assert
        AssertException<DependencyInjectionException>(action, innerMessage, outerMessage);
    }

    [Fact]
    public void ValidResolvingTypeAndDependencyType_ShouldConstructResolvingObject()
    {
        // Arrange
        ObjectConstructor objectConstructor = new();
        ConstructorInfo constructorInfo = typeof(Class1).GetConstructor(BindingFlags.Instance | BindingFlags.Public, [typeof(string)])!;
        string parameter = "unit test";

        // Act
        IClass1 actual = objectConstructor.Construct<IClass1>(constructorInfo, [parameter], EmptyKey);

        // Assert
        actual
            .Should()
            .NotBeNull()
            .And
            .BeOfType<Class1>();
        actual.BuiltBy
            .Should()
            .Be(parameter);
    }
}