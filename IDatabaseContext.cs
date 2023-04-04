using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LiteHelper;

/// <summary>
/// Context to persist data.
/// </summary>
public interface IDatabaseContext
{
    /// <summary>
    /// Insert item in collection.
    /// </summary>
    /// <param name="item">item to insert.</param>
    /// <typeparam name="T">collection.</typeparam>
    void InsertItem<T>(T item)
        where T : class;

    /// <summary>
    /// Insert items in collection.
    /// </summary>
    /// <param name="items">items to insert.</param>
    /// <typeparam name="T">collection.</typeparam>
    void InsertItems<T>(IEnumerable<T> items)
        where T : class;

    /// <summary>
    /// Upsert item in collection.
    /// </summary>
    /// <param name="item">item to upsert.</param>
    /// <typeparam name="T">collection.</typeparam>
    void UpsertItem<T>(T item)
        where T : class;

    /// <summary>
    /// Upsert items in collection.
    /// </summary>
    /// <param name="items">items to upsert.</param>
    /// <typeparam name="T">collection.</typeparam>
    void UpsertItems<T>(IEnumerable<T> items)
        where T : class;

    /// <summary>
    /// Update item in collection.
    /// </summary>
    /// <param name="item">item to update.</param>
    /// <typeparam name="T">collection.</typeparam>
    /// <returns>indicator if successful.</returns>
    bool UpdateItem<T>(T item)
        where T : class;

    /// <summary>
    /// Delete item in collection.
    /// </summary>
    /// <param name="id">item id to delete.</param>
    /// <typeparam name="T">collection.</typeparam>
    /// <returns>indicator if item was deleted successfully.</returns>
    bool DeleteItem<T>(int id)
        where T : class;

    /// <summary>
    /// Delete all items matching criteria.
    /// </summary>
    /// <param name="predicate">criteria to delete item.</param>
    /// <typeparam name="T">collection.</typeparam>
    /// <returns>number of documents deleted.</returns>
    int DeleteItems<T>(Expression<Func<T, bool>> predicate)
        where T : class;

    /// <summary>
    /// Get item by id.
    /// </summary>
    /// <param name="id">item id.</param>
    /// <typeparam name="T">collection.</typeparam>
    /// <returns>item.</returns>
    T? GetItem<T>(int id)
        where T : class;

    /// <summary>
    /// Get first item found matching criteria.
    /// </summary>
    /// <param name="predicate">criteria to return item.</param>
    /// <typeparam name="T">collection.</typeparam>
    /// <returns>item.</returns>
    T? GetItem<T>(Expression<Func<T, bool>> predicate)
        where T : class;

    /// <summary>
    /// Get all items.
    /// </summary>
    /// <typeparam name="T">collection.</typeparam>
    /// <returns>items.</returns>
    IEnumerable<T> GetItems<T>()
        where T : class;

    /// <summary>
    /// Get all items matching criteria.
    /// </summary>
    /// <param name="predicate">criteria to return item.</param>
    /// <typeparam name="T">collection.</typeparam>
    /// <returns>items.</returns>
    IEnumerable<T> GetItems<T>(Expression<Func<T, bool>> predicate)
        where T : class;

    /// <summary>
    /// Rebuilds index.
    /// </summary>
    /// <param name="predicate">expression to use.</param>
    /// <param name="unique">if field for index is unique.</param>
    /// <typeparam name="T">collection.</typeparam>
    void RebuildIndex<T>(Expression<Func<T, object>> predicate, bool unique = false)
        where T : class;

    /// <summary>
    /// Rebuild database.
    /// </summary>
    void RebuildDatabase();

    /// <summary>
    /// Create collection with no documents if doesn't exist.
    /// </summary>
    /// <typeparam name="T">collection type.</typeparam>
    void CreateCollection<T>()
        where T : new();

    /// <summary>
    /// Apply index to collection with bson expression.
    /// </summary>
    /// <param name="keySelector">predicate to determine property.</param>
    /// <typeparam name="T">collection type.</typeparam>
    /// <typeparam name="TK">type key.</typeparam>
    void EnsureIndex<T, TK>(Expression<Func<T, TK>> keySelector);

    /// <summary>
    /// Get user version.
    /// </summary>
    /// <returns>user version.</returns>
    int GetVersion();

    /// <summary>
    /// Set user version.
    /// </summary>
    /// <param name="version">user version.</param>
    void SetVersion(int version);

    /// <summary>
    /// Dispose service and terminate db connection.
    /// </summary>
    void Dispose();
}
