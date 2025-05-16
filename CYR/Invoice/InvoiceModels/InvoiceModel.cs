using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Clients;
using CYR.Invoice.InvoiceViewModels;
using CYR.Settings;

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
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; } 


        public UserSettings Seller { get; set; }
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
        public string? CommentsBottom { get; set; }
        public ImageSource Logo { get; set; }

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
