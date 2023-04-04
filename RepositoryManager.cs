using System;
using System.Collections.Generic;

namespace LiteHelper;

/// <summary>
/// Repository Manager to handle global functions across individual repos.
/// </summary>
public class RepositoryManager
{
    private readonly List<IManagedDataRepository> repositories = new ();
    private readonly List<Action> callbacks = new ();

    /// <summary>
    /// Register managed repository.
    /// </summary>
    /// <param name="repository">data repository.</param>
    public void Register(IManagedDataRepository repository)
    {
        this.repositories.Add(repository);
    }

    /// <summary>
    /// Register callback to run after queues are processed (once).
    /// </summary>
    /// <param name="callback">function to invoke.</param>
    public void Register(Action callback)
    {
        this.callbacks.Add(callback);
    }

    /// <summary>
    /// Process queues for each repository.
    /// </summary>
    public void ProcessQueues()
    {
        foreach (var repository in this.repositories)
        {
            repository.ProcessQueues();
        }

        foreach (var callback in this.callbacks)
        {
            callback.Invoke();
        }

        this.callbacks.Clear();
    }
}
