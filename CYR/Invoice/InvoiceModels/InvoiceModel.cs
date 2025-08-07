using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Clients;
using CYR.Invoice.InvoiceViewModels;
using CYR.Settings;
using CYR.User;

namespace CYR.Invoice.InvoiceModels
{
    public partial class InvoiceModel : ObservableRecipient
    {
        public InvoiceModel()
        {
            IsSelected = false;
        }
        public int? InvoiceNumber { get; set; }
        public DateTime? IssueDate { get; set; } 
        public DateTime? DueDate { get; set; } 

        public UserCompany Seller { get; set; }
        public Client Customer { get; set; }
        public decimal? NetAmount { get; set; }
        public decimal? GrossAmount { get; set; }
        [ObservableProperty]
        private InvoiceState _state;
        [ObservableProperty]
        private bool _isMwstApplicable;

        public List<InvoicePosition> Items { get; set; }

        [ObservableProperty]
        private string? _commentsTop;
        [ObservableProperty]
        private string? _commentsBottom;

        [ObservableProperty]
        private bool? _isSelected;

        partial void OnIsMwstApplicableChanged(bool oldValue, bool newValue)
        {
            if (oldValue != newValue)
            {
                Messenger.Send(new InvoiceMwstEvent(newValue));
            }
        }
    }
}
