namespace LiteHelper;

/// <summary>
/// Data Service Type to use with Data Service.
/// </summary>
public interface IRecord
{
    /// <summary>
    /// Gets or sets iD of instance.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets created date.
    /// </summary>
    public long Created { get; set; }

    /// <summary>
    /// Gets or sets last updated date.
    /// </summary>
    public long Updated { get; set; }
}
