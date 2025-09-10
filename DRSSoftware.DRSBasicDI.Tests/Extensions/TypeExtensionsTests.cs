namespace DRSSoftware.DRSBasicDI.Extensions;

using System.Globalization;
using System.Reflection;

public sealed class TypeExtensionsTests
{
    private readonly BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    [Fact]
    public void GetDIConstructorInfoForValueType_ShouldThrowException()
    {
        // Arrange
        Type type = typeof(int);
        string typeName = GetResolvingName(type);
        string expected = string.Format(CultureInfo.InvariantCulture, MsgNoSuitableConstructors, typeName);

        // Act
        void action() => type.GetDIConstructorInfo();

        // Assert
        AssertException<InvalidOperationException>(action, expected);
    }

    [Fact]
    public void GetDIConstructorInfoWithAttribute_ShouldReturnConstructorInfoWithAttribute()
    {
        // Arrange
        Type type = typeof(ManyConstructors);
        ConstructorInfo expected = type.GetConstructor(_bindingFlags, [typeof(string), typeof(string)])!;

        // Act
        ConstructorInfo actual = type.GetDIConstructorInfo();

        // Assert
        actual
            .Should()
            .BeSameAs(expected);
    }

    [Fact]
    public void GetDIConstructorInfoWithoutAttribute_ShouldReturnConstructorInfoWithMostParameters()
    {
        // Arrange
        Type type = typeof(Class1);
        ConstructorInfo expected = type.GetConstructor(_bindingFlags, [typeof(int), typeof(string)])!;

        // Act
        ConstructorInfo actual = type.GetDIConstructorInfo();

        // Assert
        actual
            .Should()
            .BeSameAs(expected);
    }

    [Theory]
    [MemberData(nameof(TestDataFactory.GetArrayTypes), MemberType = typeof(TestDataFactory))]
    public void GetFriendlyNameForArrayTypes_ShouldGenerateFriendlyName(Type type, string expected)
    {
        // Arrange/Act
        string actual = type.GetFriendlyName();

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [MemberData(nameof(TestDataFactory.GetGenericTypes), MemberType = typeof(TestDataFactory))]
    public void GetFriendlyNameForGenericTypes_ShouldGenerateFriendlyName(Type type, string expected)
    {
        // Arrange/Act
        string actual = type.GetFriendlyName();

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [MemberData(nameof(TestDataFactory.GetNullableTypes), MemberType = typeof(TestDataFactory))]
    public void GetFriendlyNameForNullableTypes_ShouldGenerateFriendlyName(Type type, string expected)
    {
        // Arrange/Act
        string actual = type.GetFriendlyName();

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [MemberData(nameof(TestDataFactory.GetPredefinedTypes), MemberType = typeof(TestDataFactory))]
    public void GetFriendlyNameForPredefinedTypes_ShouldGenerateFriendlyName(Type type, string expected)
    {
        // Arrange/Act
        string actual = type.GetFriendlyName();

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void GetFriendlyNameForSimpleClassType_ShouldGenerateFriendlyName()
    {
        // Arrange
        Type type = typeof(Class1);
        string expected = nameof(Class1);

        // Act
        string actual = type.GetFriendlyName();

        // Assert
        actual
            .Should()
            .Be(expected);
    }
}