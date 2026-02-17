
using CYR.OrderItems;
using Microsoft.Win32;
using System.IO;

namespace CYR.Services;

public class ArticleImportService : IArticleImportService
{
    public List<OrderItemCsvImport> Import()
    {
        string fileName;
        List<OrderItemCsvImport> data = [];
        OpenFileDialog fileDialog = new OpenFileDialog();
        fileDialog.Filter = "CSV | *.csv";
        fileDialog.Multiselect = false;

        bool? succes = fileDialog.ShowDialog();
        if (succes == true)
        {
            string path = fileDialog.FileName;
            fileName = fileDialog.SafeFileName;

            data = [.. File.ReadAllLines(path)
                        .Select(x => x.Split(';'))
                        .Select(dataRow => new OrderItemCsvImport
                        (
                        int.TryParse(dataRow[0]?.ToString(), out int val) ? val : null,
                        dataRow[1],
                        dataRow[2],
                        double.TryParse(dataRow[3]?.ToString(), out double dval) ? dval : null)
                        )];
        }
        return data;
    }
}
