namespace DRSSoftware.DRSBasicDI;

/// <summary>
/// An enumeration of valid dependency lifetime values.
/// </summary>
public enum DependencyLifetime
{
    /// <summary>
    /// The dependency lifetime is undefined.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The dependency lifetime is singleton.
    /// </summary>
    Singleton = 1,

    /// <summary>
    /// The dependency lifetime is scoped.
    /// </summary>
    Scoped = 2,

    /// <summary>
    /// The dependency lifetime is transient.
    /// </summary>
    Transient = 3
}