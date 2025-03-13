using CYR.Invoice.InvoiceModels;
using System.Windows;
using System.Windows.Controls;

namespace CYR.Invoice.Resources.DataTemplates
{
    public class InvoiceListViewInvoiceStateTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OpenTemplate { get; set; }
        public DataTemplate ClosedTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var state = (item as InvoiceModel)?.State;
            return state == InvoiceState.Open ? OpenTemplate : ClosedTemplate;
        }
    }
}
