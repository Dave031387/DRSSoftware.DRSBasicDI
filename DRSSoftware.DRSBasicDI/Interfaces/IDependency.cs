namespace DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="IDependency" /> interface defines the properties and methods for a dependency
/// object.
/// </summary>
internal interface IDependency : IEquatable<IDependency>
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
    /// Gets the optional factory method having no parameters which is used for creating instances
    /// of the resolving object for this <see cref="IDependency" /> object.
    /// </summary>
    public Func<object>? Factory0
    {
        get;
        init;
    }

    /// <summary>
    /// Gets the optional factory method having one parameter which is used for creating instances
    /// of the resolving object for this <see cref="IDependency" /> object.
    /// </summary>
    public Func<object, object>? Factory1
    {
        get;
        init;
    }

    /// <summary>
    /// Gets the optional factory method having two parameters which is used for creating instances
    /// of the resolving object for this <see cref="IDependency" /> object.
    /// </summary>
    public Func<object, object, object>? Factory2
    {
        get;
        init;
    }

    /// <summary>
    /// Gets the optional factory method having three parameters which is used for creating
    /// instances of the resolving object for this <see cref="IDependency" /> object.
    /// </summary>
    public Func<object, object, object, object>? Factory3
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

    /// <summary>
    /// Generate a hash code for this <see cref="IDependency" /> object.
    /// </summary>
    /// <returns>
    /// The generated hash code for this <see cref="IDependency" /> object.
    /// </returns>
    /// <remarks>
    /// The generated hash code is based on the <see cref="DependencyType" /> and <see cref="Key" />
    /// properties since those two properties uniquely identify the <see cref="IDependency" />
    /// object.
    /// </remarks>
    int GetHashCode();
}