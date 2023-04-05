using System;

namespace LiteHelper.Attribute;

/// <summary>
/// Set a name to use for the bson document collection.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class BsonCollectionAttribute : System.Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BsonCollectionAttribute"/> class.
    /// </summary>
    /// <param name="name">collection name.</param>
    public BsonCollectionAttribute(string name) => this.Name = name;

    /// <summary>
    /// Gets or sets collection name.
    /// </summary>
    public string Name { get; set; }
}
