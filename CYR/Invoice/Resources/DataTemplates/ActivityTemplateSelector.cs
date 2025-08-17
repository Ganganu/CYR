using System.Windows;
using System.Windows.Controls;
using CYR.Dashboard;

namespace CYR.Invoice.Resources.DataTemplates;

public class ActivityTemplateSelector : DataTemplateSelector
{
    public DataTemplate? InvoiceTemplate { get; set; }
    public DataTemplate? CustomerTemplate { get; set; }
    public DataTemplate? ProductTemplate { get; set; }
    public DataTemplate? AddressTemplate { get; set; }
    public DataTemplate? DefaultTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is ActivityModel activity)
        {
            return activity.ActivityType switch
            {
                "InvoiceCreated" or "InvoiceUpdated" => InvoiceTemplate ?? DefaultTemplate,
                "CustomerCreated" or "CustomerUpdated" => CustomerTemplate ?? DefaultTemplate,
                "ProductCreated" => ProductTemplate ?? DefaultTemplate,
                "AddressCreated" => AddressTemplate ?? DefaultTemplate,
                _ => DefaultTemplate
            } ?? DefaultTemplate;
        }

        return DefaultTemplate ?? base.SelectTemplate(item, container);
    }
}