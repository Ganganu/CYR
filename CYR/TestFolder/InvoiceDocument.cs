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
        public Client Client { get; }

        public InvoiceDocument(InvoiceModel model,Client client)
        {
            Model = model;
            Client = client;
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
                    column.Item().Text($"{Client.ClientName}")
                        .FontSize(11);
                    column.Item().Text($"{Client.Address}")
                        .FontSize(11);
                    column.Item().Text($"{Client.Address} {Client.Address}")
                        .FontSize(11);

                });
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