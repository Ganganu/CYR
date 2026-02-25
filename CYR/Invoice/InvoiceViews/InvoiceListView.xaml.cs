using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.Invoice.InvoiceViews;

public partial class InvoiceListView : UserControl
{
    public InvoiceListView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }

    private void StatusButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (sender is Button button && button.ContextMenu != null)
        {
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            button.ContextMenu.IsOpen = true;
            button.ContextMenu.Width = button.ActualWidth;
        }
    }
}
