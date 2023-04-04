using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LiteHelper;

/// <summary>
/// Data Repository.
/// </summary>
/// <typeparam name="T">Data type.</typeparam>
public interface IDataRepository<T>
    where T : class, IRecord, new()
{
    /// <summary>
    /// Create object.
    /// </summary>
    /// <param name="item">instance to add to collection.</param>
    /// <returns>created item.</returns>
    T Create(T item);

    /// <summary>
    /// Create objects.
    /// </summary>
    /// <param name="items">instance to add to collection.</param>
    /// <returns>created items.</returns>
    List<T> Create(List<T> items);

    /// <summary>
    /// Retrieve by id.
    /// </summary>
    /// <param name="id">int id.</param>
    /// <returns>instance.</returns>
    T? Get(int id);

    /// <summary>
    /// Retrieve records in collection by expression.
    /// </summary>
    /// <param name="exp">expression to retrieve.</param>
    /// <returns>list of instances.</returns>
    IEnumerable<T> Get(Expression<Func<T, bool>> exp);

    /// <summary>
    /// Retrieve all records in collection.
    /// </summary>
    /// <returns>list of instances.</returns>
    IEnumerable<T> GetAll();

    /// <summary>
    /// Update instance in collection.
    /// </summary>
    /// <param name="item">instance.</param>
    /// <returns>updated item.</returns>
    T Update(T item);

    /// <summary>
    /// Delete instance in collection.
    /// </summary>
    /// <param name="item">instance.</param>
    void Delete(T item);

    /// <summary>
    /// Delete all records in collection.
    /// Note: Not optimized for production use.
    /// </summary>
    void DeleteAll();

    /// <summary>
    /// Get max row id.
    /// </summary>
    /// <returns>max row id.</returns>
    int MaxId();

    /// <summary>
    /// Dispose.
    /// </summary>
    void Dispose();

    /// <summary>
    /// Process any messages in queues.
    /// </summary>
    void ProcessQueues();
}
