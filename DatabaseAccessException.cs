using System;

namespace LiteHelper;

/// <summary>
/// Exception when database file is inuse and not available for read/write.
/// </summary>
public class DatabaseAccessException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseAccessException"/> class.
    /// </summary>
    /// <param name="message">message.</param>
    /// <param name="innerException">original exception.</param>
    public DatabaseAccessException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
