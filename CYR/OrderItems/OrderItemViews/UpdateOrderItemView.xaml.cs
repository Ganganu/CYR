using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.OrderItems.OrderItemViews;

/// <summary>
/// Interaktionslogik für UpdateOrderItemView.xaml
/// </summary>
public partial class UpdateOrderItemView : UserControl
{
    public UpdateOrderItemView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
