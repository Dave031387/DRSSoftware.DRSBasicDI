namespace DRSSoftware.DRSBasicDI.Interfaces;

using System;

/// <summary>
/// The <see cref="IDependencyListConsumer" /> interface defines the properties and methods needed
/// for retrieving information from the dependency list.
/// </summary>
internal interface IDependencyListConsumer
{
    /// <summary>
    /// Get the <see cref="IDependency" /> object for the given dependency type
    /// <typeparamref name="TDependency" /> and <paramref name="key" /> value.
    /// </summary>
    /// <typeparam name="TDependency">
    /// The type of dependency to be retrieved.
    /// </typeparam>
    /// <param name="key">
    /// An optional key used to identify the specific <see cref="IDependency" /> object to be
    /// retrieved.
    /// </param>
    /// <returns>
    /// The <see cref="IDependency" /> instance corresponding to the given dependency type
    /// <typeparamref name="TDependency" /> and <paramref name="key" /> value.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    public IDependency Get<TDependency>(string key) where TDependency : class;

    /// <summary>
    /// Get the <see cref="IDependency" /> object for the given <paramref name="dependencyType" />
    /// and <paramref name="key" /> value.
    /// </summary>
    /// <param name="dependencyType">
    /// The type of dependency to be retrieved.
    /// </param>
    /// <param name="key">
    /// An optional key used to identify the specific <see cref="IDependency" /> object to be
    /// retrieved.
    /// </param>
    /// <returns>
    /// The <see cref="IDependency" /> instance corresponding to the given
    /// <paramref name="dependencyType" /> and <paramref name="key" /> value.
    /// </returns>
    /// <exception cref="DependencyInjectionException" />
    public IDependency Get(Type dependencyType, string key);
}