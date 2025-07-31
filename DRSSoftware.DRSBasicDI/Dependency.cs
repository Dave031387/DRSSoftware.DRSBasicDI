namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="Dependency" /> record represents a single dependency in an application for which
/// we want to use dependency injection.
/// </summary>
/// <param name="DependencyType">
/// Gets the dependency type of this <see cref="Dependency" /> object.
/// </param>
/// <param name="Factory">
/// Gets the optional factory method used for creating instances of the resolving object for
/// this <see cref="Dependency" /> object.
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
                                  Func<object>? Factory,
                                  string Key) : IDependency
{
    public virtual bool Equals(Dependency? other) => other is not null
                                                     && other.DependencyType == DependencyType
                                                     && other.ResolvingType == ResolvingType
                                                     && other.Lifetime == Lifetime
                                                     && (other.Factory == Factory || (other.Factory is null && Factory is null))
                                                     && other.Key == Key;

    public override int GetHashCode() => HashCode.Combine(DependencyType, ResolvingType, Lifetime, Factory, Key);
}