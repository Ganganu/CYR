using CYR.Clients;
using CYR.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CYR.TestFolder
{
    public class InvoiceDocument : IDocument
    {
        public InvoiceModel Model { get; }

        public InvoiceDocument(InvoiceModel model)
        {
            Model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    

                    page.Footer().Element(ComposeFooter);
                });
        }

        void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

            container.Row(row =>
            {
                row.RelativeItem().AlignBottom().Column(column =>
                {
                    column.Item().Text($"{Model.SellerAddress.Name}, " +
                        $"{Model.SellerAddress.Street} " +
                        $"{Model.SellerAddress.HouseNumber}," +
                        $"{Model.SellerAddress.PLZ} " +
                        $"{Model.SellerAddress.City}")
                        .FontSize(9)
                        .FontColor(Colors.Blue.Medium)
                        .Underline(true);
                });

                row.ConstantItem(200).Image("Ressources/IGF.png");
            });
            

        }
        void ComposeContent(IContainer container)
        {
            container.AlignTop().PaddingVertical(10).Row(row =>
            {
                row.RelativeItem().AlignBottom().Column(column =>
                {
                    column.Item().Element(ComposeClientInformations);

                    column.Item().Element(ComposeCommentsTop);

                    column.Item().Element(ComposeTable);
                    var totalPrice = Model.Items.Sum(x => x.Price * x.Quantity);
                    column.Item().AlignRight().Text($"Grand total: {totalPrice}$").FontSize(14);

                    column.Item().Element(ComposeComments);
                });
            });
        }
        void ComposeCommentsTop(IContainer container)
        {
            container.Background(Colors.Grey.Lighten3).PaddingTop(10).Column(column =>
            {
                column.Spacing(5);
                column.Item().Text($"Rechnung {DateTime.Parse(DateTime.Now.ToString()).Year}-1").FontSize(12).Bold();
                column.Item().Text(@"(bitte bei Bezahlung abgeben)").FontSize(8);
                column.Item().Text("Betreff: ").FontSize(12).Bold();
                column.Item().Text("Sehr geehrter Damen und Herren,").FontSize(9);
                column.Item().Text("wir danken Ihnen für den Auftrag und erlauben uns Ihnen folgende Leistungen in Rechnung zu stellen.").FontSize(9);
            });
        }
        void ComposeClientInformations(IContainer container)
        {
            container.AlignTop().PaddingVertical(10).Row(row =>
            {
                row.RelativeItem().AlignBottom().Column(column =>
                {
                    column.Item().Text($"{Model.CustomerAddress.Name}")
                        .FontSize(11);
                    column.Item().Text($"{Model.CustomerAddress.Street}")
                        .FontSize(11);
                    column.Item().Text($"{Model.CustomerAddress.PLZ} {Model.CustomerAddress.City}")
                        .FontSize(11);
                });
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"Kundennummer: {Model.CustomerAddress.ClientNumber}")
                        .FontSize(11);
                    column.Item().Text($"Datum: {DateTime.Now.ToString()}")
                        .FontSize(11);
                });
            });
        }
        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                // step 1
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                // step 2
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#");
                    header.Cell().Element(CellStyle).Text("Product");
                    header.Cell().Element(CellStyle).AlignRight().Text("Unit price");
                    header.Cell().Element(CellStyle).AlignRight().Text("Quantity");
                    header.Cell().Element(CellStyle).AlignRight().Text("Total");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                // step 3
                foreach (var item in Model.Items)
                {
                    table.Cell().Element(CellStyle).Text(Model.Items.IndexOf(item) + 1);
                    table.Cell().Element(CellStyle).Text(item.Name);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price}$");
                    table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price * item.Quantity}$");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                }
            });
        }
        void ComposeComments(IContainer container)
        {
            container.Background(Colors.Grey.Lighten3).PaddingTop(20).Column(column =>
            {
                column.Spacing(5);
                column.Item().Text("Bauleistung im Sinne von §13b Abs. 5 UStG").FontSize(11);
                column.Item().Text("Wir bitten um Überweisung des Rechnungsbetrages sofort ohne Abzug.").FontSize(11);
                column.Item().Text("");
                column.Item().Text("Mit freundlichen Grüßen").FontSize(11);
                column.Item().Text(Model.SellerAddress.Name).FontSize(11);
            });
        }
        void ComposeFooter(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"{Model.SellerAddress.Name}")
                        .FontSize(10);
                    column.Item().Text($"{Model.SellerAddress.Street} {Model.SellerAddress.HouseNumber}")
                        .FontSize(10);
                    column.Item().Text($"{Model.SellerAddress.PLZ} {Model.SellerAddress.City}")
                        .FontSize(10);

                });
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"{Model.SellerAddress.Telefonnumber}")
                        .FontSize(10);
                    column.Item().Text($"{Model.SellerAddress.EmailAddress}")
                        .FontSize(10);
                });
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"{Model.SellerAddress.BankName}")
                        .FontSize(10);
                    column.Item().Text($"{Model.SellerAddress.IBAN}")
                        .FontSize(10);
                    column.Item().Text($"{Model.SellerAddress.BIC}")
                        .FontSize(10);
                    column.Item().Text($"{Model.SellerAddress.USTIDNr}")
                        .FontSize(10);
                    column.Item().Text($"{Model.SellerAddress.STNR}")
                        .FontSize(10);
                });
            });
        }
    }
}