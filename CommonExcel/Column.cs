using System;
using System.Collections.Generic;
using System.Text;

namespace CommonExcel
{
    [AttributeUsage(AttributeTargets.All)]
    public class Column : System.Attribute
    {
        public int ColumnIndex { get; set; }
        public bool AllowNull { get; set; }

        public Column(int column, bool allowNull = true)
        {
            ColumnIndex = column;
            AllowNull = allowNull;
        }
    }
}
