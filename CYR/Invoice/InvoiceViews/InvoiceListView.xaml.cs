using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.Invoice.InvoiceViews;

/// <summary>
/// Interaktionslogik für GetInvoiceView.xaml
/// </summary>
public partial class InvoiceListView : UserControl
{
    public InvoiceListView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
