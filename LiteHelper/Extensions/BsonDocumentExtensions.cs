using System;

using LiteDB;

namespace LiteHelper.Extensions;

/// <summary>
/// BsonDocument extensions.
/// </summary>
public static class BsonDocumentExtensions
{
    /// <summary>
    /// Gets the value associated with the specified key or the default value if the key is not found or the value is null.
    /// </summary>
    /// <typeparam name="T">The type of the value to get.</typeparam>
    /// <param name="bsonDocument">The BsonDocument to get the value from.</param>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="defaultValue">The default value to return if the key is not found or the value is null.</param>
    /// <returns>The value associated with the specified key or the default value if the key is not found or the value is null.</returns>
    public static T GetValueOrDefault<T>(this BsonDocument bsonDocument, string key, T defaultValue = default!)
    {
        if (bsonDocument == null)
        {
            throw new ArgumentNullException(nameof(bsonDocument));
        }

        if (bsonDocument.TryGetValue(key, out var bsonValue))
        {
            try
            {
                var convertedValue = Convert.ChangeType(bsonValue.RawValue, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
                if (convertedValue is T value)
                {
                    return value;
                }
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
