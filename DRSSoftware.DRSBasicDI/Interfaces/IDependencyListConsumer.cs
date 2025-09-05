namespace DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="IDependencyListConsumer" /> interface defines the methods needed for retrieving
/// <see cref="IDependency" /> objects from the dependency list.
/// </summary>
internal interface IDependencyListConsumer
{
    /// <summary>
    /// Get the <see cref="IDependency" /> object associated with the given
    /// <paramref name="serviceKey" />.
    /// </summary>
    /// <param name="serviceKey">
    /// The dependency <see cref="ServiceKey" /> used to retrieve the desired
    /// <see cref="IDependency" /> object.
    /// </param>
    /// <returns>
    /// The <see cref="IDependency" /> instance corresponding to the given
    /// <paramref name="serviceKey" /> value.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    IDependency Get(ServiceKey serviceKey);
}