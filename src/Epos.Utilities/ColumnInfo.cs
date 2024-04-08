namespace Epos.Utilities;

/// <summary> Provides information details about an arbitrary table column. </summary>
public sealed class ColumnInfo
{
    /// <summary>Gets or sets the column seperator.</summary>
    /// <value>Column seperator</value>
    public ColumnSeperator? Seperator { get; set; }

    /// <summary>Gets or sets the header.</summary>
    /// <value>Header</value>
    public string Header { get; set; } = string.Empty;

    /// <summary>Gets or sets the secondary header.</summary>
    /// <value>Secondary header</value>
    public string? SecondaryHeader { get; set; }

    /// <summary>Gets or sets the datatype.</summary>
    /// <value>Datatype</value>
    public string? DataType { get; set; }

    /// <summary>Gets or sets the right alignment.</summary>
    /// <value><b>true</b>, if the column is aligned right, otherwise <b>false</b></value>
    public bool AlignRight { get; set; }
}
