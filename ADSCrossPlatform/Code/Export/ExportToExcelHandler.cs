using ClosedXML.Excel;

namespace ADS.Code.Export
{
    public class ExportToExcelHandler
    {
        public void ExportAllocationToExcel(List<Allocation> allData, string filePath)
        {
            var data = allData.Select(entry => new
            {
                Article = entry.Article,
                ProductName = entry.Name,
                StockFirst = entry.StockFirst,
                StockSecond = entry.StockSecond
            }).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Export");

                // Заголовки
                worksheet.Cell(1, 1).Value = "Article";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Polustrovsky";
                worksheet.Cell(1, 4).Value = "ChyornayaRechka";

                // Форматирование заголовков
                var headerRange = worksheet.Range(1, 1, 1, 4); // Диапазон: первая строка, от столбца 1 до 10
                headerRange.Style.Fill.BackgroundColor = XLColor.Black; // Заливка черным цветом
                headerRange.Style.Font.FontColor = XLColor.White;       // Белый цвет шрифта
                headerRange.Style.Font.Bold = true;                    // Сделать текст жирным
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Выравнивание текста по центру

                // Расширение второго столбца
                worksheet.Column(1).Width = 15;
                worksheet.Column(2).Width = 60;
                worksheet.Column(3).Width = 15;
                worksheet.Column(4).Width = 15;

                // Данные
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = data[i].Article;
                    worksheet.Cell(i + 2, 2).Value = data[i].ProductName;
                    worksheet.Cell(i + 2, 3).Value = data[i].StockFirst;
                    worksheet.Cell(i + 2, 4).Value = data[i].StockSecond;
                }

                workbook.SaveAs(filePath);
            }
        }

        public void ExportPrenoteToExcel(List<PrenoteModel> allData, string filePath)
        {
            var data = allData.Select(entry => new
            {
                Article = entry.Article,
                ProductName = entry.ProductName,
                ZoneRowPlaceLevel = $"{entry.Zone}-{entry.Row}-{entry.Place}-{entry.Level}",
                Qty = entry.Qty,
                KeyValue = entry.OrderedQty
            }).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Export");

                // Заголовки
                worksheet.Cell(1, 1).Value = "Article";
                worksheet.Cell(1, 2).Value = "ProductName";
                worksheet.Cell(1, 3).Value = "Place";
                worksheet.Cell(1, 4).Value = "Capacity";
                worksheet.Cell(1, 5).Value = "Ordered";

                // Форматирование заголовков
                var headerRange = worksheet.Range(1, 1, 1, 5); // Диапазон: первая строка, от столбца 1 до 10
                headerRange.Style.Fill.BackgroundColor = XLColor.Black; // Заливка черным цветом
                headerRange.Style.Font.FontColor = XLColor.White;       // Белый цвет шрифта
                headerRange.Style.Font.Bold = true;                    // Сделать текст жирным
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Выравнивание текста по центру

                // Данные
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = data[i].Article;
                    worksheet.Cell(i + 2, 2).Value = data[i].ProductName;
                    worksheet.Cell(i + 2, 3).Value = data[i].ZoneRowPlaceLevel;
                    worksheet.Cell(i + 2, 4).Value = data[i].Qty;
                    worksheet.Cell(i + 2, 5).Value = data[i].KeyValue;
                }

                workbook.SaveAs(filePath);
            }
        }

        public void ExportSG010ToExcel(SG010Report allData, string filePath)
        {
            var data = allData.Data.Select(entry => new
            {
                Article = entry.Article,
                ProductName = entry.ProductName,
                Store = entry.StoreID,
                Zone = entry.Zone,
                Row = entry.Row,
                Place = entry.Place,
                Level = entry.Level,
                Qty = entry.Qty,
                IsPrimary = entry.IsPrimaryPlace ? "Yes" : "No",
                IsSales = entry.IsSalesLocation ? "Yes" : "No"
            }).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Export");

                // Заголовки
                worksheet.Cell(1, 1).Value = "Article";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Store";
                worksheet.Cell(1, 4).Value = "Zone";
                worksheet.Cell(1, 5).Value = "Row";
                worksheet.Cell(1, 6).Value = "Place";
                worksheet.Cell(1, 7).Value = "Level";
                worksheet.Cell(1, 8).Value = "Qty";
                worksheet.Cell(1, 9).Value = "PrimaryLocation";
                worksheet.Cell(1, 10).Value = "SalesLocation";

                // Форматирование заголовков
                var headerRange = worksheet.Range(1, 1, 1, 10); // Диапазон: первая строка, от столбца 1 до 10
                headerRange.Style.Fill.BackgroundColor = XLColor.Black; // Заливка черным цветом
                headerRange.Style.Font.FontColor = XLColor.White;       // Белый цвет шрифта
                headerRange.Style.Font.Bold = true;                    // Сделать текст жирным
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Выравнивание текста по центру


                // Данные
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = data[i].Article;
                    worksheet.Cell(i + 2, 2).Value = data[i].ProductName;
                    worksheet.Cell(i + 2, 3).Value = data[i].Store;
                    worksheet.Cell(i + 2, 4).Value = data[i].Zone;
                    worksheet.Cell(i + 2, 5).Value = data[i].Row;
                    worksheet.Cell(i + 2, 6).Value = data[i].Place;
                    worksheet.Cell(i + 2, 7).Value = data[i].Level;
                    worksheet.Cell(i + 2, 8).Value = data[i].Qty;
                    worksheet.Cell(i + 2, 9).Value = data[i].IsPrimary;
                    worksheet.Cell(i + 2, 10).Value = data[i].IsSales;
                }

                workbook.SaveAs(filePath);
            }
        }
    }
}
