using System.Collections.Generic;

namespace Epos.Utilities
{
     /// <summary> Provides information about an arbitrary table. </summary>
   public abstract class TableInfoProvider<TColumn, TRow>
    {
        /// <summary>Gets the columns of the table.</summary>
        /// <returns>Columns</returns>
        public abstract IEnumerable<TColumn> GetColumns();

        /// <summary>Gets the column infos for the table columns.</summary>
        /// <returns>Column infos</returns>
        public abstract ColumnInfo GetColumnInfo(TColumn column, int columnIndex);

        /// <summary>Gets the rows of the table.</summary>
        /// <returns>Rows</returns>
        public abstract IEnumerable<TRow> GetRows();

        /// <summary>Gets the value of a specific table cell.</summary>
        /// <returns>Cell value and column span</returns>
        public abstract (string CellValue, int ColSpan) GetCellValue(TRow row, TColumn column, int columnIndex);

        ///<summary>Gets the number of leading spaces.</summary>
        public virtual int LeadingSpaces => 0;

        ///<summary>Determines, whether to print a sum seperator line above the last row.</summary>
        public virtual bool HasSumRow => false;

        ///<summary>Determines, whether to print a trailing seperator line under the table.</summary>
        public virtual bool HasTrailingSeperatorLine => false;

        ///<summary>Gets the text for an empty table (can be <b>null</b>).</summary>
        public virtual string? EmptyTableText => null;
    }
}
