namespace DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="IContainer" /> interface defines the methods and properties for a simple
/// dependency injection container for managing and resolving non-scoped dependencies.
/// </summary>
/// <remarks>
/// This interface inherits from the <see cref="IContainerBase" /> interface which in turn inherits
/// from <see cref="IDisposable" />.
/// </remarks>
public interface IContainer : IContainerBase
{
    /// <summary>
    /// Gets a <see langword="string"/> value used to identify this specific <see cref="IContainer" /> instance.
    /// </summary>
    public string ContainerKey
    {
        get;
    }

    /// <summary>
    /// Create a new <see cref="IScope" /> object to be used in managing a dependency scope.
    /// </summary>
    /// <returns>
    /// A new <see cref="IScope" /> object.
    /// </returns>
    public IScope CreateScope();
}