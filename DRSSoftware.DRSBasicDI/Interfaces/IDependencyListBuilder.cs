namespace DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="IDependencyListBuilder" /> interface defines the properties and methods needed
/// for maintaining the list of <see cref="IDependency" /> objects.
/// </summary>
internal interface IDependencyListBuilder
{
    /// <summary>
    /// Get the number of dependencies in the container.
    /// </summary>
    public int Count
    {
        get;
    }

    /// <summary>
    /// Add the given <paramref name="dependency" /> to the list of dependencies.
    /// </summary>
    /// <param name="dependency">
    /// The <see cref="IDependency" /> object to be added to the list of dependencies.
    /// </param>
    /// <exception cref="ContainerBuildException" />
    public void Add(IDependency dependency);
}