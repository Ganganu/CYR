using CommunityToolkit.Mvvm.ComponentModel;

namespace CYR.Invoice.InvoiceViewModels
{
    public partial class InvoiceActionsViewModel : ObservableRecipient
    {
        public InvoiceActionsViewModel()
        {
            InvoiceState = new List<string>
            {
                "Offen",
                "Geschlossen"
            };
        }

        [ObservableProperty]
        private List<string>? _invoiceState;

    }
}
