using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.Dashboard.DashboardViews;

/// <summary>
/// Interaktionslogik für DashboardInvoiceView.xaml
/// </summary>
public partial class DashboardInvoiceView : UserControl
{
    public DashboardInvoiceView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
