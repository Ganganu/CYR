using CYR.Clients;
using CYR.Model;

namespace CYR.Invoice
{
    public class InvoiceModel
    {
        public int InvoiceNumber { get; set; }
        public string IssueDate { get; set; }
        public string DueDate { get; set; }


        public User.User SellerAddress { get; set; }
        public Client CustomerAddress { get; set; }

        public List<InvoicePosition> Items { get; set; }
        public string Comments { get; set; }
    }
}
