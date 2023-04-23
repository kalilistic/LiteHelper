using System;

using LiteDB;

namespace LiteHelper.Extensions;

/// <summary>
/// BsonDocument extensions.
/// </summary>
public static class BsonDocumentExtensions
{
    /// <summary>
    /// Gets the value of the specified key in the BsonDocument, or returns the default value if the key is not found or the value cannot be cast to the requested type.
    /// </summary>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <param name="bsonDocument">The BsonDocument to retrieve the value from.</param>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="defaultValue">The default value to return if the key is not found or the value cannot be cast to the requested type. Default is the default value of the type T.</param>
    /// <returns>The value of the specified key if found and can be cast to the requested type, or the default value otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the bsonDocument parameter is null.</exception>
    public static T? GetValueOrDefault<T>(this BsonDocument bsonDocument, string key, T? defaultValue = default)
    {
        if (bsonDocument == null)
        {
            throw new ArgumentNullException(nameof(bsonDocument));
        }

        if (bsonDocument.TryGetValue(key, out var bsonValue))
        {
            try
            {
                return (T)Convert.ChangeType(bsonValue.RawValue, typeof(T));
            }
            catch (InvalidCastException)
            {
                // Ignored, return default value
            }
        }

        return defaultValue;
    }

    /// <summary>
    /// Ensures that the specified key exists in the BsonDocument and its value is not null or empty.
    /// </summary>
    /// <param name="bsonDocument">The BsonDocument to check for the key.</param>
    /// <param name="key">The key of the value to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when the bsonDocument parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the key is not found in the bsonDocument or its value is null or empty.</exception>
    public static void ValidateKeyHasValue(this BsonDocument bsonDocument, string key)
    {
        if (bsonDocument == null)
        {
            throw new ArgumentNullException(nameof(bsonDocument));
        }

        if (!bsonDocument.TryGetValue(key, out var bsonValue))
        {
            throw new ArgumentException($"Key '{key}' not found in the BsonDocument.", nameof(key));
        }

        if (bsonValue.IsNull || (bsonValue.IsString && string.IsNullOrEmpty(bsonValue.AsString)))
        {
            throw new ArgumentException($"The value for the key '{key}' is null or empty.", nameof(key));
        }
    }
}
