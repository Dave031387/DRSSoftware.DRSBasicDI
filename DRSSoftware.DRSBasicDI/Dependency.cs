namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="Dependency" /> record contains properties that describe a single dependency
/// within an application.
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
/// <param name="ResolvingKey">
/// Gets a unique resolving key value used for resolving dependencies having more than one defined
/// implementation.
/// </param>
internal record Dependency(Type DependencyType,
                                    Type ResolvingType,
                                    DependencyLifetime Lifetime,
                                    string ResolvingKey) : IDependency
{
    /// <summary>
    /// Gets the dependency <see cref="ServiceKey" /> for this <see cref="Dependency" /> object.
    /// </summary>
    public ServiceKey DependencyServiceKey => new(DependencyType, ResolvingKey);

    /// <summary>
    /// Gets the resolving <see cref="ServiceKey" /> for this <see cref="Dependency" /> object.
    /// </summary>
    public ServiceKey ResolvingServiceKey => new(ResolvingType, ResolvingKey);

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
    /// <paramref name="other" /> object; otherwise, <see langword="false" />
    /// </returns>
    public virtual bool Equals(IDependency? other) => other is not null
                                                      && other.DependencyType == DependencyType
                                                      && other.ResolvingType == ResolvingType
                                                      && other.Lifetime == Lifetime
                                                      && other.ResolvingKey == ResolvingKey;

    /// <summary>
    /// Generate a hash code for this <see cref="Dependency" /> object.
    /// </summary>
    /// <returns>
    /// The generated hash code for this <see cref="Dependency" /> object.
    /// </returns>
    /// <remarks>
    /// The generated hash code is based on the <see cref="DependencyType" /> and
    /// <see cref="ResolvingKey" /> properties since those two properties uniquely identify the
    /// <see cref="Dependency" /> object.
    /// </remarks>
    public override int GetHashCode() => HashCode.Combine(DependencyType, ResolvingKey);
}