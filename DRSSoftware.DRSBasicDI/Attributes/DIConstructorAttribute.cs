namespace DRSSoftware.DRSBasicDI.Attributes;

/// <summary>
/// Indicates that the attributed constructor should be used for dependency injection.
/// </summary>
/// <remarks>
/// This attribute is applied to a constructor to specify it as the preferred constructor for
/// dependency injection. Only one constructor in a class should be marked with this attribute to
/// avoid ambiguity. Also, the only parameters allowed on the attributed constructor are those
/// representing dependency objects that can be resolved by the dependency injection container.
/// </remarks>
[AttributeUsage(AttributeTargets.Constructor)]
public sealed class DIConstructorAttribute : Attribute
{
}