using System.Collections.Generic;

namespace Cotizaciones_API.Interfaces.Utils
{
    public interface IExcelExporter
    {
        byte[] ExportToExcel<T>(IEnumerable<T> rows, string sheetName = "Report", string? title = null);

        byte[] ExportToExcel(System.Data.DataTable table, string sheetName = "Report", string? title = null);
    }
}
