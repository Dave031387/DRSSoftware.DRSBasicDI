namespace DRSSoftware.DRSBasicDI;

using DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="ServiceKey" /> class is used as the key for retrieving <see cref="IDependency" />
/// objects and singleton or scoped instances from the dependency injection container.
/// </summary>
/// <param name="type">
/// Either a dependency type (for retrieving <see cref="IDependency" /> objects) or a resolving type
/// (for retrieving singleton or scoped instances).
/// </param>
/// <param name="key">
/// An optional key used to identify the specific object to be retrieved, or an empty string if no
/// key is needed.
/// </param>
internal sealed class ServiceKey(Type type, string key) : IEquatable<ServiceKey>
{
    /// <summary>
    /// Get the key for the <see cref="ServiceKey" /> object.
    /// </summary>
    internal string Key
    {
        get;
    } = key;

    /// <summary>
    /// Get the type for the <see cref="ServiceKey" /> object.
    /// </summary>
    internal Type Type
    {
        get;
    } = type;

    /// <summary>
    /// Get the <see cref="ServiceKey" /> corresponding to the dependency type of the given
    /// <paramref name="dependency" />.
    /// </summary>
    /// <param name="dependency">
    /// The <see cref="IDependency" /> object for which we want to get the corresponding
    /// <see cref="ServiceKey" />.
    /// </param>
    /// <returns>
    /// The <see cref="ServiceKey" /> corresponding to the dependency type of the given
    /// <paramref name="dependency" />
    /// </returns>
    internal static ServiceKey GetServiceDependencyKey(IDependency dependency)
        => GetServiceKey(dependency.DependencyType, dependency.Key);

    /// <summary>
    /// Get the <see cref="ServiceKey" /> corresponding to the given type <typeparamref name="T" />
    /// and <paramref name="key" /> values.
    /// </summary>
    /// <typeparam name="T">
    /// The type component of the <see cref="ServiceKey" /> object.
    /// </typeparam>
    /// <param name="key">
    /// The key component of the <see cref="ServiceKey" /> object.
    /// </param>
    /// <returns>
    /// The requested <see cref="ServiceKey" /> object.
    /// </returns>
    internal static ServiceKey GetServiceKey<T>(string key) where T : class
        => GetServiceKey(typeof(T), key);

    /// <summary>
    /// Get the <see cref="ServiceKey" /> corresponding to the given <paramref name="type" /> and
    /// <paramref name="key" /> values.
    /// </summary>
    /// <param name="type">
    /// The type component of the <see cref="ServiceKey" /> object.
    /// </param>
    /// <param name="key">
    /// The key component of the <see cref="ServiceKey" /> object.
    /// </param>
    /// <returns>
    /// The requested <see cref="ServiceKey" /> object.
    /// </returns>
    internal static ServiceKey GetServiceKey(Type type, string key)
        => new(type, key);

    /// <summary>
    /// Get the <see cref="ServiceKey" /> corresponding to the resolving type of the given
    /// <paramref name="dependency" />.
    /// </summary>
    /// <param name="dependency">
    /// The <see cref="IDependency" /> object for which we want to get the corresponding
    /// <see cref="ServiceKey" />.
    /// </param>
    /// <returns>
    /// The <see cref="ServiceKey" /> corresponding to the resolving type of the given
    /// <paramref name="dependency" />
    /// </returns>
    internal static ServiceKey GetServiceResolvingKey(IDependency dependency)
        => GetServiceKey(dependency.ResolvingType, dependency.Key);

    /// <summary>
    /// Determine whether the given <paramref name="other" /> <see cref="ServiceKey" /> object is
    /// equal to this object.
    /// </summary>
    /// <param name="other">
    /// The other <see cref="ServiceKey" /> object to be compared to this object.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the given <paramref name="other" /> <see cref="ServiceKey" />
    /// object is equal to this object. Otherwise, returns <see langword="false" />.
    /// </returns>
    public bool Equals(ServiceKey? other) => other is not null && other.Type == Type && other.Key == Key;

    /// <summary>
    /// Determine whether the given <paramref name="obj" /> object is equal to this object.
    /// </summary>
    /// <param name="obj">
    /// The other object to be compared to this object.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the given <paramref name="obj" /> object is equal to this object.
    /// Otherwise, returns <see langword="false" />.
    /// </returns>
    public override bool Equals(object? obj) => Equals(obj as ServiceKey);

    /// <summary>
    /// Get the hash code for the <see cref="ServiceKey" /> object.
    /// </summary>
    /// <returns>
    /// The hash code for the <see cref="ServiceKey" /> object.
    /// </returns>
    public override int GetHashCode() => HashCode.Combine(Type, Key);
}