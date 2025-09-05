namespace DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="IDependency" /> interface defines the properties that describe a singe dependency
/// within an application.
/// </summary>
internal interface IDependency : IEquatable<IDependency>
{
    /// <summary>
    /// Gets the dependency <see cref="ServiceKey" /> for this <see cref="IDependency" /> object.
    /// </summary>
    ServiceKey DependencyServiceKey
    {
        get;
    }

    /// <summary>
    /// Gets the dependency type of this <see cref="IDependency" /> object.
    /// </summary>
    public Type DependencyType
    {
        get;
        init;
    }

    /// <summary>
    /// Gets the <see cref="DependencyLifetime" /> enumeration value representing the dependency's
    /// lifetime.
    /// </summary>
    public DependencyLifetime Lifetime
    {
        get;
        init;
    }

    /// <summary>
    /// Gets a unique resolving key value used for resolving dependencies having more than one
    /// defined implementation.
    /// </summary>
    public string ResolvingKey
    {
        get;
        init;
    }

    /// <summary>
    /// Gets the resolving <see cref="ServiceKey" /> for this <see cref="IDependency" /> object.
    /// </summary>
    ServiceKey ResolvingServiceKey
    {
        get;
    }

    /// <summary>
    /// Gets the resolving type that is mapped to the <see cref="DependencyType" /> property.
    /// </summary>
    public Type ResolvingType
    {
        get;
        init;
    }

    /// <summary>
    /// Generate a hash code for this <see cref="IDependency" /> object.
    /// </summary>
    /// <returns>
    /// The generated hash code for this <see cref="IDependency" /> object.
    /// </returns>
    /// <remarks>
    /// The generated hash code is based on the <see cref="DependencyType" /> and
    /// <see cref="ResolvingKey" /> properties since those two properties uniquely identify the
    /// <see cref="IDependency" /> object.
    /// </remarks>
    int GetHashCode();
}