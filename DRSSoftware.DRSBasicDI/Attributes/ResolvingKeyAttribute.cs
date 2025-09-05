namespace DRSSoftware.DRSBasicDI.Attributes;

/// <summary>
/// Attribute used to specify the key to use when resolving a dependency.
/// </summary>
/// <param name="resolvingKey">
/// A <see langword="string" /> value that is used as the resolving key."
/// </param>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
public sealed class ResolvingKeyAttribute(string resolvingKey) : Attribute
{
    /// <summary>
    /// Get the resolving key value.
    /// </summary>
    public string Value
    {
        get;
    } = resolvingKey ?? EmptyKey;
}