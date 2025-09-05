namespace DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="IDependencyListConsumer" /> interface defines the methods needed for retrieving
/// <see cref="IDependency" /> objects from the dependency list.
/// </summary>
internal interface IDependencyListConsumer
{
    /// <summary>
    /// Get the <see cref="IDependency" /> object associated with the given
    /// <typeparamref name="TDependency" /> type and <paramref name="resolvingKey" />.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of dependency to be retrieved.
    /// </typeparam>
    /// <param name="resolvingKey">
    /// An optional key that uniquely identifies the dependency to be retrieved.
    /// </param>
    /// <returns>
    /// The <see cref="IDependency" /> object corresponding to the specified
    /// <typeparamref name="TDependency" /> type and <paramref name="resolvingKey" />.
    /// </returns>
    public IDependency Get<TDependency>(string resolvingKey) where TDependency : class;
}