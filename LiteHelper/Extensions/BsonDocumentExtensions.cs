using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using LiteDB;
using LiteHelper.BsonMapper;

namespace LiteHelper.Extensions;

using System.Globalization;

/// <summary>
/// BsonDocument extensions.
/// </summary>
public static class BsonDocumentExtensions
{
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

        if (bsonValue.IsNull || bsonValue.IsString && string.IsNullOrEmpty(bsonValue.AsString))
        {
            throw new ArgumentException($"The value for the key '{key}' is null or empty.", nameof(key));
        }
    }

    /// <summary>
    /// Converts the BsonDocument to a string representation.
    /// </summary>
    /// <param name="bsonDocument">The BsonDocument to convert.</param>
    /// <returns>A string representation of the BsonDocument.</returns>
    public static string ToDebugString(this BsonDocument? bsonDocument)
    {
        if (bsonDocument == null)
        {
            return "BsonDocument is null";
        }

        var sb = new StringBuilder();

        foreach (var element in bsonDocument)
        {
            sb.AppendLine($"{element.Key} = {element.Value}");
        }

        return sb.ToString();
    }

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
                if (typeof(T) == typeof(Vector4) ||
                    typeof(T).IsGenericType && Nullable.GetUnderlyingType(typeof(T)) == typeof(Vector4))
                {
                    var array = bsonValue.AsArray;
                    if (array != null && array.Count == 4)
                    {
                        return (T)(object)new Vector4(
                            (float)array[0].AsDouble,
                            (float)array[1].AsDouble,
                            (float)array[2].AsDouble,
                            (float)array[3].AsDouble);
                    }
                }

                if (typeof(T).IsClass && typeof(T) != typeof(string))
                {
                    if (bsonValue.IsDocument)
                    {
                        var mapper = CustomBsonMapper.Create();
                        return mapper.ToObject<T>(bsonValue.AsDocument);
                    }
                }

                if (typeof(T).IsGenericType)
                {
                    var genTypeDef = typeof(T).GetGenericTypeDefinition();
                    if (genTypeDef == typeof(Nullable<>))
                    {
                        var underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                        return (T)Convert.ChangeType(bsonValue.RawValue, underlyingType);
                    }

                    if (genTypeDef == typeof(List<>))
                    {
                        return HandleListType(bsonValue, defaultValue);
                    }
                }
                else
                {
                    return (T)Convert.ChangeType(bsonValue.RawValue, typeof(T));
                }
            }
            catch (InvalidCastException)
            {
                // Ignored, return default value
            }
        }

        return defaultValue;
    }

    private static T HandleListType<T>(BsonValue bsonValue, T defaultValue)
    {
        var elementType = typeof(T).GetGenericArguments()[0];
        var bsonArray = bsonValue.AsArray;

        if (bsonArray == null)
        {
            return defaultValue;
        }

        if (elementType == typeof(string))
        {
            return (T)(object)bsonArray.Select(x => x.AsString).ToList();
        }

        if (elementType == typeof(byte[]))
        {
            return (T)(object)bsonArray.Select(x =>
                    x == null ? null : x.IsBinary ? x.AsBinary : x.AsArray?.Select(v => (byte)v.AsInt32).ToArray())
                .ToList();
        }

        if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
        {
            return HandleKeyValuePairList<T>(bsonArray);
        }

        return HandleOtherListTypes<T>(bsonArray);
    }

    private static T HandleKeyValuePairList<T>(BsonArray bsonArray)
    {
        var elementType = typeof(T).GetGenericArguments()[0];
        var keyType = elementType.GetGenericArguments()[0];
        var valueType = elementType.GetGenericArguments()[1];

        var listType = typeof(List<>).MakeGenericType(elementType);
        var list = (IList)Activator.CreateInstance(listType) !;

        foreach (var item in bsonArray)
        {
            var keyElement = Convert.ChangeType(
                item["Key"].RawValue,
                Nullable.GetUnderlyingType(keyType) ?? keyType,
                CultureInfo.InvariantCulture);
            var valueElement = Convert.ChangeType(item["Value"].RawValue, Nullable.GetUnderlyingType(valueType) ?? valueType, CultureInfo.InvariantCulture);
            var kvp = Activator.CreateInstance(elementType, keyElement, valueElement);
            list.Add(kvp);
        }

        return (T)list;
    }

    private static T HandleOtherListTypes<T>(BsonArray bsonArray)
    {
        var elementType = typeof(T).GetGenericArguments()[0];
        var listType = typeof(List<>).MakeGenericType(elementType);
        var list = (IList)Activator.CreateInstance(listType) !;

        foreach (var item in bsonArray)
        {
            var convertedItem =
                Convert.ChangeType(item.RawValue, Nullable.GetUnderlyingType(elementType) ?? elementType);
            list.Add(convertedItem);
        }

        if (typeof(T).IsArray)
        {
            var array = Array.CreateInstance(elementType, list.Count);
            list.CopyTo(array, 0);
            return (T)(object)array;
        }

        return (T)list;
    }
}
