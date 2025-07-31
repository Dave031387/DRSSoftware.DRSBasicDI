namespace DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="IDependency" /> interface defines the properties and methods for a dependency
/// object.
/// </summary>
internal interface IDependency
{
    /// <summary>
    /// Gets the dependency type of this <see cref="IDependency" /> object.
    /// </summary>
    public Type DependencyType
    {
        get;
        init;
    }

    /// <summary>
    /// Gets the optional factory method used for creating instances of the resolving object for
    /// this <see cref="IDependency" /> object.
    /// </summary>
    public Func<object>? Factory
    {
        get;
        init;
    }

    /// <summary>
    /// Gets the optional key that can be used to identify the dependency.
    /// </summary>
    public string Key
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
    /// Gets the resolving type that is mapped to the <see cref="DependencyType" /> property.
    /// </summary>
    public Type ResolvingType
    {
        get;
        init;
    }
}