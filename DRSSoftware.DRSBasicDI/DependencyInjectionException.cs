namespace DRSSoftware.DRSBasicDI;

/// <summary>
/// The <see cref="DependencyInjectionException" /> class is intended for reporting exceptions that
/// are thrown while trying to resolve and inject dependencies.
/// </summary>
[Serializable]
public class DependencyInjectionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyInjectionException" /> class.
    /// </summary>
    public DependencyInjectionException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyInjectionException" /> class with the
    /// specified error message.
    /// </summary>
    public DependencyInjectionException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyInjectionException" /> class with the
    /// specified error message and a reference to the inner exception that is the cause of this
    /// exception.
    /// </summary>
    public DependencyInjectionException(string message, Exception inner) : base(message, inner)
    {
    }
}