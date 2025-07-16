using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.View;

/// <summary>
/// Interaktionslogik für CreateClientView.xaml
/// </summary>
public partial class CreateClientView : UserControl
{
    public CreateClientView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
