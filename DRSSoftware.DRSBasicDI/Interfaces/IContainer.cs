namespace DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="IContainer" /> interface defines the methods and properties for a simple
/// dependency injection container.
/// </summary>
public interface IContainer : IContainerBase
{
    /// <summary>
    /// Create a new <see cref="IScope" /> object to be used in managing a dependency scope.
    /// </summary>
    /// <returns>
    /// A new <see cref="IScope" /> object.
    /// </returns>
    public IScope CreateScope();
}