namespace DRSSoftware.DRSBasicDI.Common;

using DRSSoftware.DRSBasicDI.Extensions;
using System.Globalization;

/// <summary>
/// The <see cref="MessageService" /> class is a static class responsible for supplying formatted
/// messages to the other components of the <see cref="DRSBasicDI" /> class library.
/// </summary>
internal static class MessageService
{
    internal const string MsgAbstractResolvingType = "The {0} for {1} must not be an abstract class type.";
    internal const string MsgCantAddToContainerAfterBuild = "Adding a dependency to the container after the container has already been built is not allowed.";
    internal const string MsgConstructorNotFound = "No constructor having the correct number of parameters could be found for {0}.";
    internal const string MsgContainerAlreadyBuilt = "Invalid attempt to create more than one DI container.";
    internal const string MsgContainerCantBeBuiltMoreThanOnce = "The container has already been built. It can't be built again.";
    internal const string MsgContainerIsEmpty = "There must be at least one valid dependency defined before the dependency container can be built.";
    internal const string MsgContainerNotBuilt = "Invalid attempt to access the DI container before it has been built.";
    internal const string MsgDependencyMappingNotFound = "No mapping exists for {0}.";
    internal const string MsgDependencyTypeAlreadySpecified = "Invalid attempt to assign dependency type more than once to {0}.";
    internal const string MsgDuplicateDependency = "The {0} has already been added to the container.";
    internal const string MsgDuplicateService = "The {0} for {1} has already been registered in the service locator.";
    internal const string MsgErrorDuringConstruction = "An exception was thrown while trying to construct an instance of {0} for {1}.";
    internal const string MsgGenericDependencyTypeIsOpen = "Generic {0} must be fully constructed.";
    internal const string MsgIncompatibleResolvingType = "The {0} is not assignable to {1}.";
    internal const string MsgInvalidDependencyType = "The {0} must be a class type or interface type.";
    internal const string MsgInvalidResolvingType = "The {0} for {1} must be a class type.";
    internal const string MsgLifetimeAlreadySpecified = "Invalid attempt to assign lifetime more than once to {0}.";
    internal const string MsgNoSuitableConstructors = "No suitable constructor could be found for {0}.";
    internal const string MsgNullContainerKey = "The container key must not be null.";
    internal const string MsgNullDependencyObject = "Unexpected null dependency object returned for {0}.";
    internal const string MsgNullDependencyType = "Invalid attempt to assign a null value to the dependency type.";
    internal const string MsgNullInstanceCreated = "A null instance was created by the service locator for {0}";
    internal const string MsgNullResolvingKey = "Invalid attempt to assign a null value to the resolving key for {0}.";
    internal const string MsgNullResolvingType = "Invalid attempt to assign a null value to the resolving type for {0}.";
    internal const string MsgResolveMethodInfoNotFound = "The attempt to retrieve the MethodInfo for the Resolve method failed.";
    internal const string MsgResolveMethodInvocationError = "An exception was thrown when invoking the Resolve method for {0}.";
    internal const string MsgResolvingGenericTypeIsOpen = "The generic {0} for {1} must be fully constructed.";
    internal const string MsgResolvingKeyAlreadySpecified = "Invalid attempt to assign a resolving key more than once to {0}.";
    internal const string MsgResolvingObjectCouldNotBeCreated = "The resolving object could not be constructed for {0}.";
    internal const string MsgResolvingObjectNotCreated = "An instance of {0} could not be constructed for {1}.";
    internal const string MsgResolvingTypeAlreadySpecified = "Invalid attempt to assign resolving type more than once to {0}.";
    internal const string MsgScopedServiceAlreadySet = "The scoped resolving objects service has already been set in the scoped dependency resolver object.";
    internal const string MsgScopedServiceIsNull = "Invalid attempt to assign a null value to the scoped resolving objects service.";
    internal const string MsgScopedServiceSameAsNonScopedService = "The scoped resolving objects service must not be the same as the non-scoped service.";
    internal const string MsgServiceNotRegistered = "No service has been registered in the Service Locator for {0}.";
    internal const string MsgUnableToConstructService = "The Service Locator was unable to construct the service for {0}.";
    internal const string MsgUnableToMakeGenericResolveMethod = "Unable to create a generic Resolve method for {0}";
    internal const string MsgUnableToObtainConstructor = "The service locator was unable to obtain the constructor for {0}";
    internal const string MsgUndefinedLifetime = "The {0} must not have an undefined lifetime.";
    internal const string MsgUnspecifiedDependencyType = "Unable to build a dependency having an unspecified dependency type.";
    internal const string MsgUnspecifiedResolvingType = "The {0} must not have an unspecified resolving type.";

    /// <summary>
    /// Format the given message with the given dependency type, resolving key and resolving type.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The dependency type to be inserted into the message.
    /// </typeparam>
    /// <param name="message">
    /// The message to be formatted.
    /// </param>
    /// <param name="resolvingKey">
    /// The resolving key to be inserted into the message.
    /// </param>
    /// <param name="resolvingType">
    /// The resolving type to be inserted into the message.
    /// </param>
    /// <returns>
    /// The formatted message.
    /// </returns>
    internal static string FormatMessage<TDependency>(string message,
                                                      string? resolvingKey = null,
                                                      Type? resolvingType = null) where TDependency : class
        => FormatMessage(message, typeof(TDependency), resolvingKey, resolvingType);

    /// <summary>
    /// Format the given message with the given dependency type, resolving key and resolving type.
    /// </summary>
    /// <param name="message">
    /// The message to be formatted.
    /// </param>
    /// <param name="dependencyType">
    /// The dependency type to be inserted into the message.
    /// </param>
    /// <param name="resolvingKey">
    /// The resolving key to be inserted into the message.
    /// </param>
    /// <param name="resolvingType">
    /// The resolving key to be inserted into the message.
    /// </param>
    /// <param name="forceDependencyName">
    /// A boolean flag indicating whether or not the dependency name should be generated even if the
    /// <paramref name="dependencyType" /> is <see langword="null" />.
    /// </param>
    /// <remarks>
    /// Messages that contain both a dependency name and a resolving name must be constructed such
    /// that the resolving name appears in the message before the dependency name.
    /// </remarks>
    /// <returns>
    /// The formatted message.
    /// </returns>
    internal static string FormatMessage(string message,
                                         Type? dependencyType = null,
                                         string? resolvingKey = null,
                                         Type? resolvingType = null,
                                         bool forceDependencyName = false)
    {
        if (forceDependencyName || dependencyType is not null)
        {
            string dependencyTypeName = GetDependencyName(dependencyType, resolvingKey);

            if (resolvingType is not null)
            {
                string resolvingTypeName = GetResolvingName(resolvingType);
                return string.Format(CultureInfo.InvariantCulture, message, resolvingTypeName, dependencyTypeName);
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture, message, dependencyTypeName);
            }
        }
        else if (resolvingType is not null)
        {
            string resolvingTypeName = GetResolvingName(resolvingType);
            return string.Format(CultureInfo.InvariantCulture, message, resolvingTypeName);
        }
        else if (resolvingKey is not null)
        {
            return string.Format(CultureInfo.InvariantCulture, message, resolvingKey);
        }
        else
        {
            return message;
        }
    }

    /// <summary>
    /// Format the dependency name string for the given dependency type name and resolving key.
    /// </summary>
    /// <param name="dependencyTypeName">
    /// The dependency type name.
    /// </param>
    /// <param name="resolvingKey">
    /// The resolving key.
    /// </param>
    /// <returns>
    /// The formatted dependency name string.
    /// </returns>
    private static string FormatDependencyName(string dependencyTypeName, string? resolvingKey)
    {
        string part1 = $"dependency type {dependencyTypeName}";
        string key = resolvingKey ?? EmptyKey;
        string part2 = string.IsNullOrEmpty(key) ? string.Empty : $" having resolving key \"{key}\"";
        return part1 + part2;
    }

    /// <summary>
    /// Format the resolving name string for the given resolving type name.
    /// </summary>
    /// <param name="resolvingTypeName">
    /// The resolving type name.
    /// </param>
    /// <returns>
    /// The formatted resolving name string.
    /// </returns>
    private static string FormatResolvingName(string resolvingTypeName)
        => $"resolving type {resolvingTypeName}";

    /// <summary>
    /// Get the dependency name string for the given dependency type and resolving key.
    /// </summary>
    /// <param name="dependencyType">
    /// The dependency type.
    /// </param>
    /// <param name="resolvingKey">
    /// The resolving key.
    /// </param>
    /// <returns>
    /// The generated dependency name string.
    /// </returns>
    private static string GetDependencyName(Type? dependencyType, string? resolvingKey)
    {
        string dependencyTypeName = dependencyType is null ? NA : dependencyType.GetFriendlyName();
        return FormatDependencyName(dependencyTypeName, resolvingKey);
    }

    /// <summary>
    /// Get the resolving name string for the given resolving type.
    /// </summary>
    /// <param name="resolvingType">
    /// The resolving type.
    /// </param>
    /// <returns>
    /// The generated resolving name string.
    /// </returns>
    private static string GetResolvingName(Type resolvingType)
    {
        string resolvingTypeName = resolvingType.GetFriendlyName();
        return FormatResolvingName(resolvingTypeName);
    }
}