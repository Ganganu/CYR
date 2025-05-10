using CYR.Invoice.InvoiceModels;
using CYR.Services;
using CYR.Settings;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace CYR.Invoice
{
    public class InvoiceDocument :  IInvoiceDocument
    {
        private const decimal MWST = 1.19m;
        public InvoiceModel Model { get; set; }

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
                    column.Item().Text($"{Model.Seller.Name}, " +
                        $"{Model.Seller.Street} " +
                        $"{Model.Seller.HouseNumber}," +
                        $"{Model.Seller.PLZ} " +
                        $"{Model.Seller.City}")
                        .FontSize(9)
                        .FontColor(Colors.Blue.Medium)
                        .Underline(true);
                });
                row.ConstantItem(200).Image(new Uri(Model.Logo.ToString()).LocalPath);
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
                    decimal? totalPrice = Model.Items.Sum(x => x.Price * x.Quantity);
                    string formattedTotalPrice = string.Format(CultureInfo.CreateSpecificCulture("de-DE"), "{0:N2}", totalPrice);
                    column.Item().AlignRight().Text($"Netto-Summe: {formattedTotalPrice}€").FontSize(14);
                    if (Model.IsMwstApplicable)
                    {
                        column.Item().AlignRight().Text($"MwSt.         19%").FontSize(14);
                        column.Item().AlignRight().Text($"Gesamtbetrag: {(totalPrice * MWST):0.00}€").FontSize(14);
                    }
                    else
                    {
                        column.Item().AlignRight().Text("").FontSize(14);
                        column.Item().AlignRight().Text($"Gesamtbetrag: {(totalPrice):0.00}€").FontSize(14);
                    }
                    column.Item().Element(ComposeComments);
                });
            });
        }
        void ComposeCommentsTop(IContainer container)
        {
            container.Background(Colors.Grey.Lighten3).PaddingTop(10).Column(column =>
            {
                column.Spacing(5);
                column.Item().Text(Model.CommentsTop);
            });
        }
        void ComposeClientInformations(IContainer container)
        {
            container.AlignTop().PaddingVertical(10).Row(row =>
            {
                row.RelativeItem().AlignBottom().Column(column =>
                {
                    column.Item().Text($"{Model.Customer.Name}")
                        .FontSize(11);
                    column.Item().Text($"{Model.Customer.Street}")
                        .FontSize(11);
                    column.Item().Text($"{Model.Customer.PLZ} {Model.Customer.City}")
                        .FontSize(11);
                });
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"Kundennummer: {Model.Customer.ClientNumber}")
                        .FontSize(11);
                    column.Item().Text($"Datum: {Model.IssueDate.Value.ToShortDateString()}")
                        .FontSize(11);
                    if (Model.StartDate.HasValue && Model.EndDate.HasValue)
                    {
                        if (!string.IsNullOrEmpty(Model.StartDate.Value.ToShortDateString()) || !string.IsNullOrEmpty(Model.EndDate.Value.ToShortDateString()))
                        {
                            column.Item().Text($"Zeitraum: {Model.StartDate.Value.ToShortDateString()} - {Model.EndDate.Value.ToShortDateString()}")
                                .FontSize(11);
                        }
                    }
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
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.ConstantColumn(150);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                // step 2
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#");
                    header.Cell().Element(CellStyle).Text("Menge");
                    header.Cell().Element(CellStyle).Text("Einheit");
                    header.Cell().Element(CellStyle).Text("Produkt");
                    header.Cell().Element(CellStyle).AlignRight().Text("Einzelpreis");
                    header.Cell().Element(CellStyle).AlignRight().Text("Gesamtpreis");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });
                int positionCoutner = 0;
                // step 3
                foreach (var item in Model.Items)
                {
                    if (item.OrderItem == null)
                    {
                        continue;
                    }
                    positionCoutner++;
                    table.Cell().Element(CellStyle).Text(positionCoutner);
                    table.Cell().Element(CellStyle).Text(item.Quantity);
                    table.Cell().Element(CellStyle).Text(item.UnitOfMeasure?.Name);
                    table.Cell().Element(CellStyle).Text(item.OrderItem.Name);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price?.ToString("N2", new CultureInfo("de-DE"))}€");
                    table.Cell().Element(CellStyle).AlignRight().Text($"{(item.Price * item.Quantity)?.ToString("N2", new CultureInfo("de-DE"))}€");

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
                column.Item().Text(Model.CommentsBottom);
                column.Spacing(5);
                column.Item().Text(Model.Seller.Name).FontSize(11);
            });
        }
        void ComposeFooter(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"{Model.Seller.Name}")
                        .FontSize(10);
                    column.Item().Text($"{Model.Seller.Street} {Model.Seller.HouseNumber}")
                        .FontSize(10);
                    column.Item().Text($"{Model.Seller.PLZ} {Model.Seller.City}")
                        .FontSize(10);

                });
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"{Model.Seller.Telefonnumber}")
                        .FontSize(10);
                    column.Item().Text($"{Model.Seller.EmailAddress}")
                        .FontSize(10);
                });
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"{Model.Seller.BankName}")
                        .FontSize(10);
                    column.Item().Text($"{Model.Seller.IBAN}")
                        .FontSize(10);
                    column.Item().Text($"{Model.Seller.BIC}")
                        .FontSize(10);
                    column.Item().Text($"{Model.Seller.USTIDNR}")
                        .FontSize(10);
                    column.Item().Text($"{Model.Seller.STNR}")
                        .FontSize(10);
                });
            });
        }
    }
}