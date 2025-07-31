namespace DRSSoftware.DRSBasicDI;

/// <summary>
/// The <see cref="ContainerBuildException" /> class is intended for reporting exceptions that are
/// thrown by the <see cref="ContainerBuilder" /> object.
/// </summary>
[Serializable]
public class ContainerBuildException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBuildException" /> class.
    /// </summary>
    public ContainerBuildException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBuildException" /> class with the
    /// specified error message.
    /// </summary>
    public ContainerBuildException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBuildException" /> class with the
    /// specified error message and a reference to the inner exception that is the cause of this
    /// exception.
    /// </summary>
    public ContainerBuildException(string message, Exception inner) : base(message, inner)
    {
    }
}