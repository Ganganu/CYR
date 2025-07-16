using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.View;

/// <summary>
/// Interaktionslogik für SettingsView.xaml
/// </summary>
public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
