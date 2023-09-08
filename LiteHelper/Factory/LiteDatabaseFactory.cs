using System;
using System.IO;
using LiteDB;
using LiteHelper.BsonMapper;
using LiteHelper.Exception;

namespace LiteHelper.Factory;

/// <summary>
/// LiteDatabaseFactory is a wrapper class that manages the lifecycle of a LiteDatabase instance.
/// </summary>
public class LiteDatabaseFactory : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiteDatabaseFactory"/> class with the specified configurations.
    /// </summary>
    /// <param name="dataFolder">The folder where the database file will be stored.</param>
    /// <param name="dbName">The name of the database file (without the file extension).</param>
    /// <param name="connectionMode">The connection mode for the LiteDatabase instance (default is "shared").</param>
    public LiteDatabaseFactory(string dataFolder, string dbName = "data", string connectionMode = "shared")
    {
        this.Database = Build(dataFolder, dbName, connectionMode);
    }

    /// <summary>
    /// Gets the LiteDatabase instance managed by this factory.
    /// </summary>
    public ILiteDatabase Database { get; }

    /// <summary>
    /// Build LiteDB database.
    /// </summary>
    /// <param name="dataFolder">The folder where the database file will be stored.</param>
    /// <param name="dbName">The name of the database file (without the file extension).</param>
    /// <param name="connectionMode">The connection mode for the LiteDatabase instance (default is "shared").</param>
    /// <returns>db instance.</returns>
    public static ILiteDatabase Build(string dataFolder, string dbName = "data", string connectionMode = "shared")
    {
        var bsonMapper = CustomBsonMapper.Create();
        Directory.CreateDirectory(dataFolder);
        var fileName = $"{dataFolder}\\{dbName}.db";
        EnsureDatabaseAccess(fileName);
        var connectionString = $"Filename={fileName};connection={connectionMode}";
        return new LiteDatabase(connectionString, bsonMapper);
    }

    /// <summary>
    /// Disposes the LiteDatabase instance managed by this factory.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        this.Database.Dispose();
    }

    /// <summary>
    /// Ensures that the database file can be accessed and throws a DatabaseAccessException if it cannot.
    /// </summary>
    /// <param name="fileName">The full path of the database file.</param>
    private static void EnsureDatabaseAccess(string fileName)
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
}
