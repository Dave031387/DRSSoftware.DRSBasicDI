namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="Dependency" /> record represents a single dependency in an application for which
/// we want to use dependency injection.
/// </summary>
/// <param name="DependencyType">
/// Gets the dependency type of this <see cref="Dependency" /> object.
/// </param>
/// <param name="Lifetime">
/// Gets the <see cref="DependencyLifetime" /> enumeration value representing the dependency's
/// lifetime.
/// </param>
/// <param name="ResolvingType">
/// Gets the resolving type that is mapped to the <see cref="DependencyType" /> property.
/// </param>
/// <param name="Key">
/// An optional key that can be used to identify the dependency.
/// </param>
internal record Dependency(Type DependencyType,
                                    Type ResolvingType,
                                    DependencyLifetime Lifetime,
                                    string Key) : IDependency
{
    /// <summary>
    /// Indicates whether the current <see cref="Dependency" /> object is equal to the specified
    /// <see cref="IDependency" /> object.
    /// </summary>
    /// <param name="other">
    /// The <see cref="IDependency" /> object to be compared to this <see cref="Dependency" />
    /// object.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if this <see cref="Dependency" /> object is equal to the
    /// <paramref name="other" /> <see cref="IDependency" /> object; otherwise,
    /// <see langword="false" />
    /// </returns>
    public virtual bool Equals(IDependency? other) => other is not null
                                                      && other.DependencyType == DependencyType
                                                      && other.ResolvingType == ResolvingType
                                                      && other.Lifetime == Lifetime
                                                      && other.Key == Key;

    /// <summary>
    /// Generate a hash code for this <see cref="Dependency" /> object.
    /// </summary>
    /// <returns>
    /// The generated hash code for this <see cref="Dependency" /> object.
    /// </returns>
    /// <remarks>
    /// The generated hash code is based on the <see cref="DependencyType" /> and <see cref="Key" />
    /// properties since those two properties uniquely identify the <see cref="Dependency" />
    /// object.
    /// </remarks>
    public override int GetHashCode() => HashCode.Combine(DependencyType, Key);
}