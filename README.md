# LiteHelper
[![Nuget](https://img.shields.io/nuget/v/LiteHelper)](https://www.nuget.org/packages/LiteHelper/)

A small helper library to use with [LiteDB](https://github.com/mbdavid/LiteDB).

## Features
- Provide BsonCollectionAttribute to get and set collection names
- Provide CustomBsonMapper with sensible defaults and support for more data types
- Provide BsonDocumentExtensions for dynamic key and value retrieval
- Provide Factory pattern for easier building of LiteDB instances
- Provide Record with comparer for easier implementation of persistence

## Example

```csharp
using var dbFactory = new LiteDatabaseFactory("D:\databaseDir", "db", "direct");
ILiteCollection<BsonDocument> collection = dbFactory.Database.GetCollection("MyCollection");
if (collection != null)
{
    var documents = collection.FindAll().ToList();
    if (documents.Count > 0)
    {
        var document = documents.First();
        var name = document.GetValueOrDefault<string>("Name");
    }
}
```
