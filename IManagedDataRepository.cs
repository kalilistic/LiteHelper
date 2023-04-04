namespace LiteHelper;

/// <summary>
/// DataRepository to managed by RepositoryManager.
/// </summary>
public interface IManagedDataRepository
{
    /// <summary>
    /// Process any messages in queues.
    /// </summary>
    void ProcessQueues();

    /// <summary>
    /// Dispose.
    /// </summary>
    void Dispose();
}
