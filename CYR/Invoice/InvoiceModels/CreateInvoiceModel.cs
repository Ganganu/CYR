using System.Collections.ObjectModel;
using System.Windows.Media;
using CYR.Clients;
using CYR.Invoice.InvoiceViewModels;

namespace CYR.Invoice.InvoiceModels
{
    public class CreateInvoiceModel
    {
        public ObservableCollection<InvoicePosition> Positions { get; set; }
        public Client Client { get; set; }
        public int? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public bool IsMwstApplicable { get; set; }
        public string? CommentsTop { get; set; }
        public string? CommentsBottom { get; set; }
        public ImageSource? Logo { get; set; }
    }
}
