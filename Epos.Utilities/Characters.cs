using System;

namespace Epos.Utilities
{
    /// <summary>Collection of special character string constants like
    /// <see cref="Lf"/> for a cross-platform linefeed or <see cref="Tab"/>.
    /// </summary>
    public static class Characters
    {
        /// <summary>Linefeed</summary>
        public static readonly string Lf = Environment.NewLine;

        /// <summary>Double linefeed</summary>
        public static readonly string DbLf = Lf + Lf;

        /// <summary>Tabulator</summary>
        public static readonly string Tab = "\t";
    }
}
