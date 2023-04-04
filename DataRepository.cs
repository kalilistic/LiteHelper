using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LiteHelper;

/// <summary>
/// Service class for methods backed by databaseContext.
/// </summary>
/// <typeparam name="T">Type.</typeparam>
public abstract class DataRepository<T> : IDataRepository<T>
    where T : class, IRecord, new()
{
    /// <summary>
    /// Context for interacting with database.
    /// </summary>
    protected IDatabaseContext databaseContext;

    /// <summary>
    /// Queue for create requests.
    /// </summary>
    private readonly ConcurrentQueue<T> createQueue = new ();

    /// <summary>
    /// Queue for read requests.
    /// </summary>
    private readonly ConcurrentQueue<T> readQueue = new ();

    /// <summary>
    /// Queue for update requests.
    /// </summary>
    private readonly ConcurrentQueue<T> updateQueue = new ();

    /// <summary>
    /// Queue for delete requests.
    /// </summary>
    private readonly ConcurrentQueue<T> deleteQueue = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="DataRepository{T}"/> class.
    /// </summary>
    /// <param name="databaseContext">database databaseContext.</param>
    protected DataRepository(IDatabaseContext databaseContext)
    {
        this.databaseContext = databaseContext;
        databaseContext.CreateCollection<T>();
    }

    /// <inheritdoc />
    public T Create(T item)
    {
        var currentTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        item.Created = currentTime;
        item.Updated = currentTime;
        this.createQueue.Enqueue(item);
        return item;
    }

    /// <inheritdoc />
    public List<T> Create(List<T> items)
    {
        var currentTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        foreach (var item in items)
        {
            item.Created = currentTime;
            item.Updated = currentTime;
            this.createQueue.Enqueue(item);
        }

        return items;
    }

    /// <inheritdoc />
    public T? Get(int id)
    {
        return this.databaseContext.GetItem<T>(id);
    }

    /// <inheritdoc />
    public IEnumerable<T> Get(Expression<Func<T, bool>> exp)
    {
        return this.databaseContext.GetItems(exp);
    }

    /// <inheritdoc />
    public IEnumerable<T> GetAll()
    {
        return this.databaseContext.GetItems<T>();
    }

    /// <inheritdoc />
    public T Update(T item)
    {
        item.Updated = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        this.updateQueue.Enqueue(item);
        return item;
    }

    /// <inheritdoc />
    public void Delete(T item)
    {
        this.deleteQueue.Enqueue(item);
    }

    /// <inheritdoc />
    public void DeleteAll()
    {
        var items = this.databaseContext.GetItems<T>();
        foreach (var item in items)
        {
            this.deleteQueue.Enqueue(item);
        }
    }

    /// <inheritdoc />
    public int MaxId()
    {
        var items = this.databaseContext.GetItems<T>().ToList();
        if (!items.Any())
        {
            return 0;
        }

        return items.Max(record => record.Id);
    }

    /// <summary>
    /// Dispose.
    /// </summary>
    public void Dispose()
    {
        this.ProcessQueues();
    }

    /// <inheritdoc />
    public void ProcessQueues()
    {
        while (!this.updateQueue.IsEmpty)
        {
            var result = this.updateQueue.TryDequeue(out var item);
            if (result && item != null)
            {
                this.databaseContext.UpdateItem(item);
            }
        }

        while (!this.createQueue.IsEmpty)
        {
            var result = this.createQueue.TryDequeue(out var item);
            if (result && item != null)
            {
                this.databaseContext.InsertItem(item);
            }
        }

        while (!this.deleteQueue.IsEmpty)
        {
            var result = this.deleteQueue.TryDequeue(out var item);
            if (result && item != null)
            {
                this.databaseContext.DeleteItem<T>(item.Id);
            }
        }
    }
}
