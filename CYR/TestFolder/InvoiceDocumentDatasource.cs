using CYR.Clients;
using CYR.Model;
using CYR.User;
using QuestPDF.Helpers;

namespace CYR.TestFolder
{
    public static class InvoiceDocumentDataSource
    {
        private static Random Random = new Random();
        public static InvoiceModel GetInvoiceDetails(Client client)
        {
            var items = Enumerable
                .Range(1, 25)
                .ToList();

            ConfigReader configReader = new ConfigReader();
            return new InvoiceModel
            {
                InvoiceNumber = Random.Next(1_000, 10_000),
                IssueDate = DateTime.Now.ToString(),
                DueDate = (DateTime.Now + TimeSpan.FromDays(14)).ToString(),

                SellerAddress = new User.User { Name = configReader.CompanyName, City = configReader.City, HouseNumber = configReader.HouseNumber, Street = configReader.Street},
                CustomerAddress = GetCustomerAddress(client),

                //Items = items,
                Comments = Placeholders.Paragraph()
            };
        }

        private static Client GetCustomerAddress(Client client)
        {
            return new Client
            {
                Name = client.Name,
                Street = client.Street,
                City = client.City,
                PLZ = client.PLZ,
                ClientNumber = client.ClientNumber
            };
        }
    }
}
