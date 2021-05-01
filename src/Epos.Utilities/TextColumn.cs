using System;
using System.Collections.Generic;

namespace Epos.Utilities
{
    internal sealed class TextColumn<TColumn>
    {
        public TextColumn(string header, int index, TColumn column, bool alignRight) {
            Header = header;
            Index = index;
            Column = column;
            Alignright = alignRight;

            Width = header.Length;

            Rows = new List<string>();
        }

        public string Header { get; }

        public int Index { get; }

        public TColumn Column { get; }

        public bool Alignright { get; }

        private string? mySecondaryHeader;
        public string? SecondaryHeader {
            get => mySecondaryHeader;
            set {
                mySecondaryHeader = value;

                if (mySecondaryHeader != null) {
                    Width = Math.Max(Width, mySecondaryHeader.Length);
                }
            }
        }

        private string? myDataType;
        public string? DataType {
            get => myDataType;
            set {
                myDataType = value;

                if (myDataType != null) {
                    Width = Math.Max(Width, myDataType.Length);
                }
            }
        }

        public int Width { get; set; }

        public List<string> Rows { get; }
    }
}
