using System.Collections.ObjectModel;
using CYR.Clients;
using CYR.Model;

namespace CYR.Invoice.Model
{
    public class CreateInvoiceModel
    {
        public ObservableCollection<InvoicePosition> Positions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Client Client { get; set; }
        public int InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? Subject { get; set; }
        public string? ObjectNumber { get; set; }
        public bool IsMwstApplicable { get; set; }


    }
}
