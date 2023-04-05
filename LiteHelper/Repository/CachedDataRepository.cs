using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using LiteHelper.Context;
using LiteHelper.Model;

namespace LiteHelper.Repository;

/// <summary>
/// Service class for methods backed by databaseContext.
/// </summary>
/// <typeparam name="T">Type.</typeparam>
public abstract class CachedDataRepository<T> : DataRepository<T>
    where T : class, IRecord, new()
{
    /// <summary>
    /// Cache the collection in-memory.
    /// </summary>
    protected Dictionary<int, T> cache = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachedDataRepository{T}"/> class.
    /// </summary>
    /// <param name="databaseContext">db databaseContext.</param>
    protected CachedDataRepository(IDatabaseContext databaseContext)
        : base(databaseContext)
    {
        this.ResetCache();
    }

    /// <summary>
    /// Reload documents from db collection.
    /// </summary>
    public void ResetCache()
    {
        this.cache = this.databaseContext.GetItems<T>().ToDictionary(item => item.Id, item => item);
    }

    /// <summary>
    /// Create object.
    /// </summary>
    /// <param name="item">instance to add to collection.</param>
    public new void Create(T item)
    {
        item = base.Create(item);
        this.cache.TryAdd(item.Id, item);
    }

    /// <summary>
    /// Retrieve by id.
    /// </summary>
    /// <param name="id">int id.</param>
    /// <returns>instance.</returns>
    public new T? Get(int id)
    {
        this.cache.TryGetValue(id, out var item);
        return item;
    }

    /// <summary>
    /// Retrieve records in collection by expression.
    /// </summary>
    /// <param name="exp">expression to retrieve.</param>
    /// <returns>list of instances.</returns>
    public new IEnumerable<T> Get(Expression<Func<T, bool>> exp)
    {
        return this.databaseContext.GetItems(exp);
    }

    /// <summary>
    /// Retrieve all records in collection.
    /// </summary>
    /// <returns>list of instances.</returns>
    public new IEnumerable<T> GetAll()
    {
        return this.cache.Values;
    }

    /// <summary>
    /// Update instance in collection.
    /// </summary>
    /// <param name="item">instance.</param>
    public new void Update(T item)
    {
        item = base.Update(item);
        this.cache[item.Id] = item;
    }

    /// <summary>
    /// Delete instance in collection.
    /// </summary>
    /// <param name="item">instance.</param>
    public new void Delete(T item)
    {
        base.Delete(item);
        this.cache.Remove(item.Id);
    }

    /// <summary>
    /// Delete all records in collection.
    /// Note: Not optimized for production use.
    /// </summary>
    public new void DeleteAll()
    {
        base.DeleteAll();
        this.cache.Clear();
    }
}
