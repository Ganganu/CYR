using CYR.Clients;
using CYR.Model;
using CYR.User;
using QuestPDF.Helpers;

namespace CYR.Invoice
{
    public static class InvoiceDocumentDataSource
    {
        private static int _invoiceNumber;
        public static void SetInvoiceNumber(int invoiceNumber)
        {
            _invoiceNumber = invoiceNumber;
        }
        public static InvoiceModel GetInvoiceDetails(Client client, IEnumerable<InvoicePosition> positions)
        {
            ConfigReader configReader = new ConfigReader();
            return new InvoiceModel
            {
                InvoiceNumber = _invoiceNumber,
                IssueDate = DateTime.Now.ToShortDateString(),
                DueDate = (DateTime.Now + TimeSpan.FromDays(14)).ToString(),

                Seller = new User.User { Name = configReader.CompanyName, City = configReader.City, HouseNumber = configReader.HouseNumber, Street = configReader.Street },
                Customer = GetCustomerAddress(client),

                
                Items = positions.ToList(),
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
