# LiteHelper
[![Nuget](https://img.shields.io/nuget/v/LiteHelper)](https://www.nuget.org/packages/LiteHelper/)

A small helper library to use with [LiteDB](https://github.com/mbdavid/LiteDB).

### Features
- Provide BsonCollectionAttribute to get and set collection names
- Provide CustomBsonMapper with sensible defaults and support for more data types
- Provide data repository pattern to abstract from LiteDB
- Provide queue system to register and apply changes

### Example

```csharp
// initialize instance
var context = new LiteDatabaseContext("D:\databaseDir");

// create collection
context.CreateCollection<Player>();

// add document
var player = new Player();
context.InsertItem(player);

// retrieve document using LINQ
context.GetItem<Player>(p => p.Id == 1);
```
