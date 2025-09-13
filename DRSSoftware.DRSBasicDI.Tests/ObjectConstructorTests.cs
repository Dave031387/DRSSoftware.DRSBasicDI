namespace DRSSoftware.DRSBasicDI;

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
}
