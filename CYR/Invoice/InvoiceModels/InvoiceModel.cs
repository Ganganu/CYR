using CYR.Clients;
using CYR.Invoice.InvoiceViewModels;
using CYR.Settings;

namespace CYR.Invoice.InvoiceModels
{
    public class InvoiceModel
    {
        public InvoiceModel()
        {
            IsSelected = false;
        }
        public int? InvoiceNumber { get; set; }
        public string? IssueDate { get; set; } 
        public string? DueDate { get; set; } 
        public string? StartDate { get; set; } 
        public string? EndDate { get; set; } 


        public UserSettings Seller { get; set; }
        public Client Customer { get; set; }
        public decimal? NetAmount { get; set; }
        public decimal? GrossAmount { get; set; }
        public string? Paragraph { get; set; }
        public InvoiceState State { get; set; }
        public string? ObjectNumber { get; set; }
        public string? Subject { get; set; }
        public bool Mwst { get; set; }

        public List<InvoicePosition> Items { get; set; }
        public string Comments { get; set; }
        public string? Notiz {  get; set; }
        public bool? IsSelected { get; set; }
    }
}
