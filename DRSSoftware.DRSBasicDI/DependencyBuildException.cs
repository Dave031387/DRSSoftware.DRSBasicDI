namespace DRSSoftware.DRSBasicDI;

/// <summary>
/// The <see cref="DependencyBuildException" /> class is intended for reporting exceptions that are
/// thrown by the <see cref="DependencyBuilder" /> object.
/// </summary>
[Serializable]
public class DependencyBuildException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyBuildException" /> class.
    /// </summary>
    public DependencyBuildException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyBuildException" /> class with the
    /// specified error message.
    /// </summary>
    public DependencyBuildException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyBuildException" /> class with the
    /// specified error message and a reference to the inner exception that is the cause of this
    /// exception.
    /// </summary>
    public DependencyBuildException(string message, Exception inner) : base(message, inner)
    {
    }
}