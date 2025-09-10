namespace DRSSoftware.DRSBasicDI.Common;

public sealed class MessageServiceTests
{
    private const string DependencyTypeIClass1 = " dependency type IClass1";
    private const string DependencyTypeNA = " dependency type " + NA;
    private const string FirstParameter = " {0}";
    private const string FirstText = "first";
    private const string MessageWithNoParameters = "message";
    private const string MessageWithOneParameter = FirstText + FirstParameter + SecondText;
    private const string MessageWithTwoParameters = MessageWithOneParameter + SecondParameter + ThirdText;
    private const string ResolvingKey = @" having resolving key ""test key""";
    private const string ResolvingType = " resolving type Class1";
    private const string SecondParameter = " {1}";
    private const string SecondText = " second";
    private const string ThirdText = " third";
    private static readonly Type _dependencyType = typeof(IClass1);
    private static readonly string _resolvingKey = "test key";
    private static readonly Type _resolvingType = typeof(Class1);

    [Fact]
    public void GenericMessageWithDependencyType_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + DependencyTypeIClass1 + SecondText;

        // Act
        string actual = FormatMessage<IClass1>(MessageWithOneParameter);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void GenericMessageWithDependencyTypeAndResolvingKey_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + DependencyTypeIClass1 + ResolvingKey + SecondText;

        // Act
        string actual = FormatMessage<IClass1>(MessageWithOneParameter, _resolvingKey);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void GenericMessageWithDependencyTypeAndResolvingKeyAndResolvingType_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + ResolvingType + SecondText + DependencyTypeIClass1 + ResolvingKey + ThirdText;

        // Act
        string actual = FormatMessage<IClass1>(MessageWithTwoParameters, _resolvingKey, _resolvingType);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void GenericMessageWithDependencyTypeAndResolvingType_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + ResolvingType + SecondText + DependencyTypeIClass1 + ThirdText;

        // Act
        string actual = FormatMessage<IClass1>(MessageWithTwoParameters, null, _resolvingType);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithDependencyType_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + DependencyTypeIClass1 + SecondText;

        // Act
        string actual = FormatMessage(MessageWithOneParameter, _dependencyType);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithDependencyTypeAndForced_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + DependencyTypeIClass1 + SecondText;

        // Act
        string actual = FormatMessage(MessageWithOneParameter, _dependencyType, null, null, true);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithDependencyTypeAndResolvingKey_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + DependencyTypeIClass1 + ResolvingKey + SecondText;

        // Act
        string actual = FormatMessage(MessageWithOneParameter, _dependencyType, _resolvingKey);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithDependencyTypeAndResolvingKeyAndForced_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + DependencyTypeIClass1 + ResolvingKey + SecondText;

        // Act
        string actual = FormatMessage(MessageWithOneParameter, _dependencyType, _resolvingKey, null, true);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithDependencyTypeAndResolvingKeyAndResolvingType_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + ResolvingType + SecondText + DependencyTypeIClass1 + ResolvingKey + ThirdText;

        // Act
        string actual = FormatMessage(MessageWithTwoParameters, _dependencyType, _resolvingKey, _resolvingType);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithDependencyTypeAndResolvingKeyAndResolvingTypeAndForced_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + ResolvingType + SecondText + DependencyTypeIClass1 + ResolvingKey + ThirdText;

        // Act
        string actual = FormatMessage(MessageWithTwoParameters, _dependencyType, _resolvingKey, _resolvingType, true);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithDependencyTypeAndResolvingType_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + ResolvingType + SecondText + DependencyTypeIClass1 + ThirdText;

        // Act
        string actual = FormatMessage(MessageWithTwoParameters, _dependencyType, null, _resolvingType);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithDependencyTypeAndResolvingTypeAndForced_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + ResolvingType + SecondText + DependencyTypeIClass1 + ThirdText;

        // Act
        string actual = FormatMessage(MessageWithTwoParameters, _dependencyType, null, _resolvingType, true);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithForcedDependencyName_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + DependencyTypeNA + SecondText;

        // Act
        string actual = FormatMessage(MessageWithOneParameter, null, null, null, true);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithNoParameters_ShouldReturnUnalteredMessage()
    {
        // Arrange
        string expected = MessageWithNoParameters;

        // Act
        string actual = FormatMessage(MessageWithNoParameters);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithResolvingKey_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + " " + _resolvingKey + SecondText;

        // Act
        string actual = FormatMessage(MessageWithOneParameter, null, _resolvingKey);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithResolvingKeyAndForcedDependencyName_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + DependencyTypeNA + ResolvingKey + SecondText;

        // Act
        string actual = FormatMessage(MessageWithOneParameter, null, _resolvingKey, null, true);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithResolvingKeyAndResolvingTypeAndForcedDependencyName_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + ResolvingType + SecondText + DependencyTypeNA + ResolvingKey + ThirdText;

        // Act
        string actual = FormatMessage(MessageWithTwoParameters, null, _resolvingKey, _resolvingType, true);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithResolvingType_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + ResolvingType + SecondText;

        // Act
        string actual = FormatMessage(MessageWithOneParameter, null, null, _resolvingType);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void MessageWithResolvingTypeAndForcedDependencyName_ShouldReturnFormattedMessage()
    {
        // Arrange
        string expected = FirstText + ResolvingType + SecondText + DependencyTypeNA + ThirdText;

        // Act
        string actual = FormatMessage(MessageWithTwoParameters, null, null, _resolvingType, true);

        // Assert
        actual
            .Should()
            .Be(expected);
    }
}