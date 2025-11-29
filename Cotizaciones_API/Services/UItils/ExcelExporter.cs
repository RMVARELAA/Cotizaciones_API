using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using ClosedXML.Excel;
using Cotizaciones_API.Interfaces.Utils;

namespace Cotizaciones_API.Services.Utils
{
    public class ExcelExporter : IExcelExporter
    {
        public byte[] ExportToExcel<T>(IEnumerable<T> rows, string sheetName = "Report", string? title = null)
        {
            var dt = ToDataTable(rows);
            return ExportToExcel(dt, sheetName, title);
        }

        public byte[] ExportToExcel(DataTable table, string sheetName = "Report", string? title = null)
        {
            if (table.Columns.Count == 0)
            {
                table.Columns.Add("Column1");
            }
            if (table.Rows.Count == 0)
            {
                table.Rows.Add(table.NewRow());
            }

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(table, sheetName);

            if (!string.IsNullOrWhiteSpace(title))
            {
                var colCount = Math.Max(1, table.Columns.Count);
                var firstRow = ws.Row(1);
                firstRow.InsertRowsAbove(1);
                ws.Cell(1, 1).Value = title;

                var rng = ws.Range(1, 1, 1, colCount);
                rng.Merge();
                rng.Style.Font.SetBold();
                rng.Style.Font.FontSize = 14;
                rng.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        public byte[] ExportListToExcel<T>(
            IEnumerable<T> rows,
            string sheetName = "Report",
            string? title = null,
            string[]? columnsOrder = null,
            Func<string, string>? headerFormatter = null)
        {
            if (rows == null) throw new ArgumentNullException(nameof(rows));

            var list = rows as IList<T> ?? rows.ToList();
            if (list.Count == 0)
            {
                return ExportToExcel(new DataTable(), sheetName, title);
            }

            var first = list.First()!;
            IEnumerable<string> allColumns;

            if (first is IDictionary<string, object> dictGen)
            {
                allColumns = dictGen.Keys;
            }
            else if (first is IReadOnlyDictionary<string, object> readOnlyDict)
            {
                allColumns = readOnlyDict.Keys;
            }
            else if (first is System.Collections.IDictionary nonGenDict)
            {
                allColumns = nonGenDict.Keys.Cast<object>().Select(k => k?.ToString() ?? "");
            }
            else
            {
                allColumns = first.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => p.Name);
            }

            var columns = (columnsOrder != null && columnsOrder.Length > 0)
                ? columnsOrder.Where(c => allColumns.Contains(c)).ToArray()
                : allColumns.ToArray();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);

            int startRow = 1;
            if (!string.IsNullOrWhiteSpace(title))
            {
                ws.Cell(startRow, 1).Value = title;
                var colCount = Math.Max(1, columns.Length);
                ws.Range(startRow, 1, startRow, colCount).Merge();
                ws.Range(startRow, 1, startRow, colCount).Style.Font.SetBold().Font.FontSize = 14;
                ws.Range(startRow, 1, startRow, colCount).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                startRow++;
            }

            int colIndex = 1;
            foreach (var col in columns)
            {
                var header = headerFormatter != null ? headerFormatter(col) : col;
                ws.Cell(startRow, colIndex).Value = header;
                ws.Cell(startRow, colIndex).Style.Font.SetBold();
                colIndex++;
            }

            int rowIndex = startRow + 1;
            foreach (var item in list)
            {
                colIndex = 1;
                foreach (var col in columns)
                {
                    object? value = null;

                    if (item is IDictionary<string, object> d1)
                    {
                        d1.TryGetValue(col, out value);
                    }
                    else if (item is IReadOnlyDictionary<string, object> d2)
                    {
                        d2.TryGetValue(col, out value);
                    }
                    else if (item is System.Collections.IDictionary d3)
                    {
                        value = d3[col];
                    }
                    else
                    {
                        var prop = item.GetType().GetProperty(col, BindingFlags.Public | BindingFlags.Instance);
                        if (prop != null)
                            value = prop.GetValue(item);
                    }

                    var cell = ws.Cell(rowIndex, colIndex);

                    // --- Asignación válida sin genéricos ---

                    if (value == null)
                    {
                        cell.Value = "";
                    }
                    else if (value is DateTime dt)
                    {
                        cell.Value = dt;
                        cell.Style.DateFormat.Format = "yyyy-MM-dd HH:mm:ss";
                    }
                    else if (value is decimal dec)
                    {
                        cell.Value = Convert.ToDouble(dec);
                        cell.Style.NumberFormat.Format = "#,##0.00";
                    }
                    else if (value is double d)
                    {
                        cell.Value = d;
                        cell.Style.NumberFormat.Format = "#,##0.00";
                    }
                    else if (value is float f)
                    {
                        cell.Value = Convert.ToDouble(f); // CORREGIDO
                        cell.Style.NumberFormat.Format = "#,##0.00";
                    }
                    else if (value is int || value is long || value is short || value is byte)
                    {
                        cell.Value = Convert.ToInt64(value);
                    }
                    else if (value is bool b)
                    {
                        cell.Value = b;
                    }
                    else
                    {
                        cell.Value = value.ToString() ?? "";
                    }

                    colIndex++;
                }

                rowIndex++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        private DataTable ToDataTable<T>(IEnumerable<T> data)
        {
            var dt = new DataTable();
            if (data == null) return dt;

            var list = data as IList<T> ?? data.ToList();
            if (list.Count == 0) return dt;

            var first = list[0];
            if (first == null) return dt;

            if (first is IDictionary<string, object> dictGen)
            {
                foreach (var key in dictGen.Keys)
                    dt.Columns.Add(key ?? "Column", typeof(object));

                foreach (IDictionary<string, object> row in list.Cast<IDictionary<string, object>>())
                {
                    var dr = dt.NewRow();
                    foreach (var key in row.Keys)
                        dr[key ?? "Column"] = row[key] ?? DBNull.Value;
                    dt.Rows.Add(dr);
                }
                return dt;
            }

            if (first is IReadOnlyDictionary<string, object> readOnlyDict)
            {
                foreach (var key in readOnlyDict.Keys)
                    dt.Columns.Add(key ?? "Column", typeof(object));

                foreach (var item in list.Cast<IReadOnlyDictionary<string, object>>())
                {
                    var dr = dt.NewRow();
                    foreach (var key in item.Keys)
                        dr[key ?? "Column"] = item.TryGetValue(key, out var v) ? v ?? DBNull.Value : DBNull.Value;
                    dt.Rows.Add(dr);
                }
                return dt;
            }

            if (first is System.Collections.IDictionary dictNonGen)
            {
                foreach (var key in dictNonGen.Keys)
                    dt.Columns.Add(key?.ToString() ?? "Column", typeof(object));

                foreach (System.Collections.IDictionary row in list)
                {
                    var dr = dt.NewRow();
                    foreach (var key in row.Keys)
                        dr[key?.ToString() ?? "Column"] = row[key] ?? DBNull.Value;
                    dt.Rows.Add(dr);
                }
                return dt;
            }

            var props = first.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (props.Length == 0)
            {
                var fields = first.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var f in fields)
                {
                    var type = Nullable.GetUnderlyingType(f.FieldType) ?? f.FieldType;
                    dt.Columns.Add(f.Name, type);
                }

                foreach (var item in list)
                {
                    var dr = dt.NewRow();
                    foreach (var f in fields)
                    {
                        var val = f.GetValue(item);
                        dr[f.Name] = val ?? DBNull.Value;
                    }
                    dt.Rows.Add(dr);
                }

                return dt;
            }

            foreach (var p in props)
            {
                var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                dt.Columns.Add(p.Name, type);
            }

            foreach (var item in list)
            {
                var dr = dt.NewRow();
                foreach (var p in props)
                {
                    var val = p.GetValue(item);
                    dr[p.Name] = val ?? DBNull.Value;
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}
