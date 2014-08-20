using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace InvestmentApi.Web.Helpers
{
    public static class NPOIHelpers
    {
        public static HSSFWorkbook OpenWorkbook(string filePath)
        {
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return new HSSFWorkbook(file);
            }
        }
        public static string GetCellText(this IRow row, int cellIndex)
        {
            return row.GetCell(cellIndex).StringCellValue.TrimText();
        }
    }
}