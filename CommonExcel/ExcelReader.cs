using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommonExcel
{
    public class ExcelReader
    {
        /// <summary>
        /// Simple rows recipient from csv-file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public List<List<string>> GetRows(string fileName, char separator)
        {
            var rows = new List<List<string>>();
            using (var reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var row = new List<string>();
                    rows.Add(line.Split(separator).ToList());
                }
            }
            return rows;
        }

        /// <summary>
        /// T must declare which field need to parse with custom attribute ColumnAttribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public List<T> ParseRows<T>(string fileName, out List<ExcelError> errors) where T : new()
        {
            var stream = File.OpenRead(fileName);
            return ParseRows<T>(fileName, stream, out errors);
        }

        public List<T> ParseRows<T>(string fileName, byte[] bytes, out List<ExcelError> errors) where T : new()
        {
            var stream = new MemoryStream(bytes);
            return ParseRows<T>(fileName, stream, out errors);
        }

        public List<T> ParseRows<T>(string fileName, Stream stream, out List<ExcelError> errors) where T : new()
        {
            var result = new List<T>();
            errors = new List<ExcelError>();
            if (fileName.EndsWith("xlsx"))
            {
                //Get the workbook instance for XLS file 
                var xworkbook = new XSSFWorkbook(stream);
                var xsheet = xworkbook.GetSheetAt(0);
                return ParseSheet<T>(xsheet, out errors);
            }
            else if (fileName.EndsWith("xls"))
            {
                var hworkbook = new HSSFWorkbook(stream);
                var hsheet = hworkbook.GetSheetAt(0);
                return ParseSheet<T>(hsheet, out errors);
            }
            return result;
        }

        private class ColumnDetails
        {
            public PropertyInfo Property { get; set; }
            public Column Column { get; set; }
        }

        private List<T> ParseSheet<T>(ISheet sheet, out List<ExcelError> errors) where T : new()
        {
            var result = new List<T>();
            errors = new List<ExcelError>();
            bool columnOnly(CustomAttributeData y) => y.AttributeType == typeof(Column);
            var from = sheet.FirstRowNum;
            var to = sheet.LastRowNum;
            var columns = typeof(T)
                .GetProperties()
                .Where(x => x.CustomAttributes.Any(columnOnly))
                .Select(p => new ColumnDetails
                {
                    Property = p,
                    Column = p.GetCustomAttributes<Column>().First() //safe because if where above
                }).ToList();
            for (int i = from; i <= to; i++)
            {
                try
                {
                    var data = ParseRow<T>(sheet.GetRow(i), columns);
                    result.Add(data);
                }
                catch (Exception e)
                {
                    var row = sheet.GetRow(i);
                    var rowString = string.Empty;
                    if (row != null)
                    {
                        try
                        {
                            string GetStringCell(ICell c)
                            {
                                c.SetCellType(CellType.String);
                                return c.StringCellValue;
                            }
                            rowString = string.Join(";", row.Cells.Select(GetStringCell));
                        }
                        catch (Exception ex)
                        {
                            rowString = "error while join row";
                        }
                    }
                    errors.Add(
                        new ExcelError
                        {
                            Error = e.ToString(),
                            RowNumber = i,
                            RowString = rowString
                        }
                    );
                }
            }
            return result;
        }

        private T ParseRow<T>(IRow row, List<ColumnDetails> columns) where T : new()
        {
            var tnew = new T();
            if (row != null)
            {
                columns.ForEach(col =>
                {
                    var cell = row.GetCell(col.Column.ColumnIndex, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    //If it is numeric it is a double since that is how excel stores all numbers

                    if (col.Property.PropertyType == typeof(int))
                    {
                        if (cell.CellType == CellType.Blank)
                        {
                            if (col.Column.AllowNull)
                            {
                                col.Property.SetValue(tnew, 0);
                            }
                            else
                            {
                                throw new Exception("Empty cell while expected int");
                            }
                        }

                        var value = cell.NumericCellValue;
                        col.Property.SetValue(tnew, (int)value);
                        return;
                    }
                    if (col.Property.PropertyType == typeof(double))
                    {
                        if (cell.CellType == CellType.Blank)
                        {
                            if (col.Column.AllowNull)
                            {
                                col.Property.SetValue(tnew, 0);
                            }
                            else
                            {
                                throw new Exception("Empty cell while expected double");
                            }
                        }
                        col.Property.SetValue(tnew, cell.NumericCellValue);
                        return;
                    }
                    if (col.Property.PropertyType == typeof(DateTime))
                    {
                        if (cell.CellType == CellType.Blank)
                        {
                            if (col.Column.AllowNull)
                            {
                                col.Property.SetValue(tnew, new DateTime(1900, 1, 1));
                            }
                            else
                            {
                                throw new Exception("Empty cell while expected DateTime");
                            }
                        }
                        col.Property.SetValue(tnew, cell.DateCellValue);
                        return;
                    }
                    if (cell == null)
                    {
                        col.Property.SetValue(tnew, string.Empty);
                    }
                    //Its a string
                    cell.SetCellType(CellType.String);
                    col.Property.SetValue(tnew, cell.StringCellValue);
                });
                return tnew;
            }
            throw new Exception("empty row");
        }


    }
}
