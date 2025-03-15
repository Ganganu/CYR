using System.ComponentModel.DataAnnotations;

namespace CYR.Invoice.InvoiceModels
{
    public enum InvoiceState
    {
        [Display(Name = "Offen")]
        Open = 0,
        [Display(Name = "Bezahlt")]
        Closed = 1
    }
}
