using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.Invoice.InvoiceViews;

/// <summary>
/// Interaktionslogik für ShowInvoiceView.xaml
/// </summary>
public partial class ShowInvoiceView : UserControl
{
    public ShowInvoiceView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
