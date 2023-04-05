namespace LiteHelper.Exception;

/// <summary>
/// Exception when database file is inuse and not available for read/write.
/// </summary>
public class DatabaseAccessException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseAccessException"/> class.
    /// </summary>
    /// <param name="message">message.</param>
    /// <param name="innerException">original exception.</param>
    public DatabaseAccessException(string? message, System.Exception? innerException)
        : base(message, innerException)
    {
    }
}
