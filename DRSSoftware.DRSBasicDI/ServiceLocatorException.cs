namespace DRSSoftware.DRSBasicDI;

/// <summary>
/// The <see cref="ServiceLocatorException" /> class is intended for reporting exceptions that are
/// thrown in the <see cref="ServiceLocator" /> class.
/// </summary>
[Serializable]
public class ServiceLocatorException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLocatorException" /> class.
    /// </summary>
    public ServiceLocatorException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLocatorException" /> class with the
    /// specified error message.
    /// </summary>
    public ServiceLocatorException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLocatorException" /> class with the
    /// specified error message and a reference to the inner exception that is the cause of this
    /// exception.
    /// </summary>
    public ServiceLocatorException(string message, Exception inner) : base(message, inner)
    {
    }
}