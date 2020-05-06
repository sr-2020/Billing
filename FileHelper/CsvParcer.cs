using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FileHelper
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

    public class CsvParcer
    {
        private class ColumnDetails
        {
            public PropertyInfo Property { get; set; }
            public Column Column { get; set; }
        }
        public List<T> ParceCsv<T>(IFormFile file) where T : new()
        {
            var result = new List<T>();
            bool columnOnly(CustomAttributeData y) => y.AttributeType == typeof(Column);
            var columns = typeof(T)
                .GetProperties()
                .Where(x => x.CustomAttributes.Any(columnOnly))
                .Select(p => new ColumnDetails
                {
                    Property = p,
                    Column = p.GetCustomAttributes<Column>().First() //safe because if where above
                }).ToList();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    line = line.Replace(";", ",");
                    var values = line.Split(',');
                    result.Add(ParceRow<T>(values.ToList(), columns));
                }
            }
            return result;
        }
        private T ParceRow<T>(List<string> values, List<ColumnDetails> columns) where T : new()
        {
            var tnew = new T();
            if (values != null)
            {
                columns.ForEach(col =>
                {
                    if (col.Column.ColumnIndex >= values.Count)
                        return;
                    var cell = values[col.Column.ColumnIndex];
                    if (col.Property.PropertyType == typeof(int))
                    {
                        var value = int.Parse(cell);
                        col.Property.SetValue(tnew, value);
                        return;
                    }
                    col.Property.SetValue(tnew, cell);
                });
                return tnew;
            }
            return tnew;
        }
    }
}
