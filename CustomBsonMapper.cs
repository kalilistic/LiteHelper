using System;
using System.Numerics;

using LiteDB;

namespace LiteHelper;

/// <summary>
/// Create custom bson mapper with defaults and additional registered types.
/// </summary>
public static class CustomBsonMapper
{
    /// <summary>
    /// Create an instance of bson mapper.
    /// </summary>
    /// <returns>instance of bson mapper.</returns>
    public static BsonMapper Create()
    {
        var bsonMapper = new BsonMapper
        {
            IncludeFields = true,
            EmptyStringToNull = false,
            SerializeNullValues = true,
            ResolveCollectionName = ResolveCollectionName,
            ResolveFieldName = ResolveFieldName,
            EnumAsInteger = true,
        };
        bsonMapper.RegisterType(
            vector4 =>
            {
                var doc = new BsonArray
                {
                    new (vector4.X),
                    new (vector4.Y),
                    new (vector4.Z),
                    new (vector4.W),
                };
                return doc;
            },
            doc => new Vector4((float)doc[0].AsDouble, (float)doc[1].AsDouble, (float)doc[2].AsDouble, (float)doc[3].AsDouble));
        bsonMapper.RegisterType(
            vector3 =>
            {
                var doc = new BsonArray
                {
                    new (vector3.X),
                    new (vector3.Y),
                    new (vector3.Z),
                };
                return doc;
            },
            (doc) => new Vector3((float)doc[0].AsDouble, (float)doc[1].AsDouble, (float)doc[2].AsDouble));
        return bsonMapper;
    }

    private static string ResolveCollectionName(Type t)
    {
        var attr = (BsonCollectionAttribute?)Attribute.GetCustomAttribute(t, typeof(BsonCollectionAttribute));
        return attr == null ? t.Name : attr.Name;
    }

    private static string ResolveFieldName(string name)
    {
        return char.ToLower(name[0]) + name[1..];
    }
}
