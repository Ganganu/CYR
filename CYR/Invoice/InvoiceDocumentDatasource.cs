using CYR.Clients;
using CYR.Invoice.InvoiceModels;
using CYR.Invoice.InvoiceViewModels;
using CYR.Settings;
using CYR.User;

namespace CYR.Invoice
{
    public static class InvoiceDocumentDataSource
    {
        private static int? _invoiceNumber;
        public static void SetInvoiceNumber(int? invoiceNumber)
        {
            _invoiceNumber = invoiceNumber;
        }
        public static InvoiceModel GetInvoiceDetails(Client client, IEnumerable<InvoicePosition> positions,InvoiceModel invoiceModel,UserSettings user)
        {
            ConfigReader configReader = new ConfigReader();
            return new InvoiceModel
            {
                InvoiceNumber = _invoiceNumber,
                IssueDate = invoiceModel.IssueDate,
                DueDate = DateTime.Now + TimeSpan.FromDays(14),

                //Seller = new User.User { Name = configReader.CompanyName, City = configReader.City, HouseNumber = configReader.HouseNumber, Street = configReader.Street },
                Seller = user,
                Customer = GetCustomerAddress(client),

                Items = [.. positions]
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
