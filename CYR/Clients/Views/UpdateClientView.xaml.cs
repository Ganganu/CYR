using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.Clients.Views;

/// <summary>
/// Interaktionslogik für UpdateClientView.xaml
/// </summary>
public partial class UpdateClientView : UserControl
{
    public UpdateClientView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
