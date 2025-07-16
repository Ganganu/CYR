using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.View;

/// <summary>
/// Interaktionslogik für ClientView.xaml
/// </summary>
public partial class ClientView : UserControl
{
    public ClientView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
