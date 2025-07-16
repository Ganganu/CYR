using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.Invoice.InvoiceViews;

/// <summary>
/// Interaktionslogik für InvoiceView.xaml
/// </summary>
public partial class CreateInvoiceView : UserControl
{
    public CreateInvoiceView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
