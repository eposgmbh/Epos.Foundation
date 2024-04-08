namespace Epos.Utilities;

/// <summary> Provides information about a column seperator.<!-- --></summary>
public sealed class ColumnSeperator
{
    /// <summary>Gets or sets the header.</summary>
    /// <value>Header</value>
    public string Header { get; set; } = string.Empty;

    /// <summary>Gets or sets the colspan.</summary>
    /// <value>Colspan</value>
    public int ColSpan { get; set; }
}
