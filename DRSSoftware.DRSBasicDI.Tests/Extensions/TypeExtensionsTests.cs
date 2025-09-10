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
        string typeName = "resolving type int";
        string expected = string.Format(CultureInfo.InvariantCulture, MsgNoSuitableConstructors, typeName);

        // Act
        void action() => type.GetDIConstructorInfo();

        // Assert
        AssertException<InvalidOperationException>(action, expected);
    }

    [Fact]
    public void GetDIConstructorInfoForValueTypeWithParameterCount_ShouldThrowException()
    {
        // Arrange
        Type type = typeof(int);
        string typeName = "resolving type int";
        int parameterCount = 1;
        string expected = string.Format(CultureInfo.InvariantCulture, MsgNoSuitableConstructors, typeName);

        // Act
        void action() => type.GetDIConstructorInfo(parameterCount);

        // Assert
        AssertException<InvalidOperationException>(action, expected);
    }

    [Fact]
    public void GetDIConstructorInfoForClassTypeWithInvalidParameterCount_ShouldThrowException()
    {
        // Arrange
        Type type = typeof(ManyConstructors);
        string typeName = "resolving type ManyConstructors";
        int parameterCount = 3;
        string expected = string.Format(CultureInfo.InvariantCulture, MsgConstructorNotFound, typeName);

        // Act
        void action() => type.GetDIConstructorInfo(parameterCount);

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
    public void GetDIConstructorInfoWithParameterCountAndAttribute_ShouldReturnConstructorInfoWithAttribute()
    {
        // Arrange
        Type type = typeof(ManyConstructors);
        int parameterCount = 2;
        ConstructorInfo expected = type.GetConstructor(_bindingFlags, [typeof(string), typeof(string)])!;

        // Act
        ConstructorInfo actual = type.GetDIConstructorInfo(parameterCount);

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

    [Fact]
    public void GetDIConstructorInfoWithParameterCountAndNoAttribute_ShouldReturnConstructorInfoWithCorrectParameterCount()
    {
        // Arrange
        Type type = typeof(Class1);
        int parameterCount = 1;
        ConstructorInfo expected1 = type.GetConstructor(_bindingFlags, [typeof(int)])!;
        ConstructorInfo expected2 = type.GetConstructor(_bindingFlags, [typeof(string)])!;
        ConstructorInfo[] expected = [expected1, expected2];

        // Act
        ConstructorInfo actual = type.GetDIConstructorInfo(parameterCount);

        // Assert
        expected
            .Should()
            .Contain(actual);
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