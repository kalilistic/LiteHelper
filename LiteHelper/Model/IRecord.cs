namespace LiteHelper.Model;

/// <summary>
/// Data Service Type to use with Data Service.
/// </summary>
public interface IRecord
{
    /// <summary>
    /// Gets iD of instance.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets or sets created date.
    /// </summary>
    public long Created { get; set; }

    /// <summary>
    /// Gets or sets last updated date.
    /// </summary>
    public long Updated { get; set; }
}
