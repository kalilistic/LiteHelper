using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

using LiteDB;
using LiteHelper.BsonMapper;
using LiteHelper.Exception;

namespace LiteHelper.Context;

/// <inheritdoc />
public class LiteDatabaseContext : IDatabaseContext
{
    private readonly ILiteDatabase database;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiteDatabaseContext"/> class.
    /// </summary>
    /// <param name="dataFolder">data folder.</param>
    /// <param name="dbName">database name excluding extension.</param>
    /// <param name="connection">connection (direct or shared).</param>
    public LiteDatabaseContext(string dataFolder, string dbName = "data", string connection = "direct")
    {
        var bsonMapper = CustomBsonMapper.Create();
        Directory.CreateDirectory(dataFolder);
        var fileName = $"{dataFolder}\\{dbName}.db";
        VerifyDatabaseAccess(fileName);
        var connectionString = $"Filename={fileName};connection={connection}";
        this.database = new LiteDatabase(connectionString, bsonMapper);
    }

    /// <inheritdoc />
    public void CreateCollection<T>()
        where T : new()
    {
        var collection = this.database.GetCollection<T>();
        if (collection.Count() == 0)
        {
            collection.Insert(new T());
            collection.DeleteAll();
        }
    }

    /// <inheritdoc />
    public void EnsureIndex<T, TK>(Expression<Func<T, TK>> keySelector)
    {
        var collection = this.database.GetCollection<T>();
        collection.EnsureIndex(keySelector);
    }

    /// <inheritdoc />
    public void InsertItem<T>(T item)
        where T : class
    {
        var collection = this.database.GetCollection<T>();
        collection.Insert(item);
    }

    /// <inheritdoc />
    public void InsertItems<T>(IEnumerable<T> items)
        where T : class
    {
        var collection = this.database.GetCollection<T>();
        var enumerable = items as T[] ?? items.ToArray();
        collection.InsertBulk(enumerable, enumerable.Length);
    }

    /// <inheritdoc />
    public void UpsertItem<T>(T item)
        where T : class
    {
        var collection = this.database.GetCollection<T>();
        collection.Upsert(item);
    }

    /// <inheritdoc />
    public void UpsertItems<T>(IEnumerable<T> items)
        where T : class
    {
        var collection = this.database.GetCollection<T>();
        collection.Upsert(items);
    }

    /// <inheritdoc />
    public bool UpdateItem<T>(T item)
        where T : class
    {
        var collection = this.database.GetCollection<T>();
        return collection.Update(item);
    }

    /// <inheritdoc />
    public bool DeleteItem<T>(int id)
        where T : class
    {
        var collection = this.database.GetCollection<T>();
        return collection.Delete(id);
    }

    /// <inheritdoc />
    public int DeleteItems<T>(Expression<Func<T, bool>> predicate)
        where T : class
    {
        var collection = this.database.GetCollection<T>();
        return collection.DeleteMany(predicate);
    }

    /// <inheritdoc />
    public T? GetItem<T>(int id)
        where T : class
    {
        var collection = this.database.GetCollection<T>();
        return collection.FindById(id);
    }

    /// <inheritdoc />
    public T? GetItem<T>(Expression<Func<T, bool>> predicate)
        where T : class
    {
        return this.InternalGetItems(predicate).FirstOrDefault();
    }

    /// <inheritdoc />
    public IEnumerable<T> GetItems<T>()
        where T : class
    {
        var collection = this.database.GetCollection<T>();
        var result = collection.FindAll();
        return result.AsEnumerable();
    }

    /// <inheritdoc />
    public IEnumerable<T> GetItems<T>(Expression<Func<T, bool>> predicate)
        where T : class
    {
        return this.InternalGetItems(predicate);
    }

    /// <inheritdoc />
    public void RebuildIndex<T>(Expression<Func<T, object>> predicate, bool unique = false)
        where T : class
    {
        var collection = this.database.GetCollection<T>();
        collection.EnsureIndex(predicate, unique);
    }

    /// <inheritdoc />
    public void RebuildDatabase()
    {
        this.database.Rebuild();
    }

    /// <inheritdoc />
    public int GetVersion()
    {
        return this.database.UserVersion;
    }

    /// <inheritdoc />
    public void SetVersion(int version)
    {
        this.database.UserVersion = version;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.database.Dispose();
    }

    /// <summary>
    /// Get collection.
    /// </summary>
    /// <param name="name">lite db collection.</param>
    /// <returns>bson collection.</returns>
    public ILiteCollection<BsonDocument>? GetCollection(string name)
    {
        if (this.database.CollectionExists(name))
        {
            return this.database.GetCollection<BsonDocument>(name);
        }

        return null;
    }

    private static void VerifyDatabaseAccess(string fileName)
    {
        try
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            using Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (System.Exception ex)
        {
            throw new DatabaseAccessException($"Can't access {fileName}", ex);
        }
    }

    private IEnumerable<T> InternalGetItems<T>(
        Expression<Func<T, bool>>? predicate)
        where T : class
    {
        var collection = this.database.GetCollection<T>();

        var result = predicate != null
                         ? collection.Find(predicate)
                         : collection.Find(Query.All());

        return result.AsEnumerable();
    }
}
