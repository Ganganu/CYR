using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CYR.OrderItems;

public class PrintOrderItemService : IPrintOrderItemService
{
    public void Print(IEnumerable<OrderItem> orderItems)
    {
        try
        {
            var flowDocument = new FlowDocument();
            flowDocument.PagePadding = new Thickness(40); // Add some padding for readability
            flowDocument.ColumnWidth = double.PositiveInfinity; // Use full width

            // Add title
            var titleParagraph = new Paragraph();
            titleParagraph.Inlines.Add(new Run("Artikel- und Dienstleistungsübersicht")
            {
                FontSize = 24,
                FontWeight = FontWeights.Bold
            });
            titleParagraph.TextAlignment = TextAlignment.Center;
            titleParagraph.Margin = new Thickness(0, 0, 0, 30);
            flowDocument.Blocks.Add(titleParagraph);

            // Create table
            var table = new Table();
            table.CellSpacing = 0;
            table.BorderBrush = Brushes.Black;
            table.BorderThickness = new Thickness(1);

            // Define columns with proper widths
            table.Columns.Add(new TableColumn { Width = new GridLength(25, GridUnitType.Star) }); // Name
            table.Columns.Add(new TableColumn { Width = new GridLength(50, GridUnitType.Star) }); // Description
            table.Columns.Add(new TableColumn { Width = new GridLength(25, GridUnitType.Star) }); // Price

            // Create header row group
            var headerRowGroup = new TableRowGroup();
            headerRowGroup.Background = Brushes.LightGray;

            var headerRow = new TableRow();
            headerRow.FontWeight = FontWeights.Bold;

            // Header cells
            var headerNameCell = new TableCell();
            headerNameCell.Padding = new Thickness(10, 8, 10, 8);
            headerNameCell.BorderBrush = Brushes.Black;
            headerNameCell.BorderThickness = new Thickness(0, 0, 1, 1);
            headerNameCell.Blocks.Add(new Paragraph(new Run("Name")) { Margin = new Thickness(0) });
            headerRow.Cells.Add(headerNameCell);

            var headerDescCell = new TableCell();
            headerDescCell.Padding = new Thickness(10, 8, 10, 8);
            headerDescCell.BorderBrush = Brushes.Black;
            headerDescCell.BorderThickness = new Thickness(0, 0, 1, 1);
            headerDescCell.Blocks.Add(new Paragraph(new Run("Beschreibung")) { Margin = new Thickness(0) });
            headerRow.Cells.Add(headerDescCell);

            var headerPriceCell = new TableCell();
            headerPriceCell.Padding = new Thickness(10, 8, 10, 8);
            headerPriceCell.BorderBrush = Brushes.Black;
            headerPriceCell.BorderThickness = new Thickness(0, 0, 0, 1);
            headerPriceCell.TextAlignment = TextAlignment.Right;
            headerPriceCell.Blocks.Add(new Paragraph(new Run("Preis")) { Margin = new Thickness(0) });
            headerRow.Cells.Add(headerPriceCell);

            headerRowGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerRowGroup);

            // Create data row group
            var dataRowGroup = new TableRowGroup();

            // Add data rows
            foreach (var item in orderItems)
            {
                var row = new TableRow();

                // Name cell
                var nameCell = new TableCell();
                nameCell.Padding = new Thickness(10, 8, 10, 8);
                nameCell.BorderBrush = Brushes.Black;
                nameCell.BorderThickness = new Thickness(0, 0, 1, 1);
                nameCell.Blocks.Add(new Paragraph(new Run(item.Name ?? ""))
                {
                    FontWeight = FontWeights.SemiBold,
                    Margin = new Thickness(0)
                });
                row.Cells.Add(nameCell);

                // Description cell
                var descCell = new TableCell();
                descCell.Padding = new Thickness(10, 8, 10, 8);
                descCell.BorderBrush = Brushes.Black;
                descCell.BorderThickness = new Thickness(0, 0, 1, 1);
                descCell.Blocks.Add(new Paragraph(new Run(item.Description ?? ""))
                {
                    Margin = new Thickness(0)
                });
                row.Cells.Add(descCell);

                // Price cell
                var priceCell = new TableCell();
                priceCell.Padding = new Thickness(10, 8, 10, 8);
                priceCell.TextAlignment = TextAlignment.Right;
                priceCell.BorderBrush = Brushes.Black;
                priceCell.BorderThickness = new Thickness(0, 0, 0, 1);

                string priceText = "";
                if (item.Price is decimal decimalPrice)
                {
                    priceText = decimalPrice.ToString("C2");
                }
                else
                {
                    priceText = item.Price?.ToString() ?? "0,00 €";
                }

                priceCell.Blocks.Add(new Paragraph(new Run(priceText))
                {
                    Margin = new Thickness(0)
                });
                row.Cells.Add(priceCell);

                dataRowGroup.Rows.Add(row);
            }

            table.RowGroups.Add(dataRowGroup);
            flowDocument.Blocks.Add(table);

            // Print the document
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
                printDialog.PrintDocument(paginator, "Order Items Overview");
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}