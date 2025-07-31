namespace DRSSoftware.DRSBasicDI;

/// <summary>
/// The <see cref="ServiceLocaterException" /> class is intended for reporting exceptions that are
/// thrown in the <see cref="ServiceLocater" /> class.
/// </summary>
[Serializable]
public class ServiceLocaterException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLocaterException" /> class.
    /// </summary>
    public ServiceLocaterException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLocaterException" /> class with the
    /// specified error message.
    /// </summary>
    public ServiceLocaterException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLocaterException" /> class with the
    /// specified error message and a reference to the inner exception that is the cause of this
    /// exception.
    /// </summary>
    public ServiceLocaterException(string message, Exception inner) : base(message, inner)
    {
    }
}