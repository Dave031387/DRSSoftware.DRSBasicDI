namespace DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// The <see cref="IScope" /> interface defines the methods and properties for managing and
/// resolving scoped dependencies. Each <see cref="IScope" /> object manages scoped dependencies
/// within a single dependency scope.
/// </summary>
/// <remarks>
/// This interface inherits from the <see cref="IContainerBase" /> interface which in turn inherits
/// from <see cref="IDisposable" />.
/// </remarks>
public interface IScope : IContainerBase
{
}