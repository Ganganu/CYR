using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace CYR.Dialog;

/// <summary>
/// Interaktionslogik für DialogWindow.xaml
/// </summary>
public partial class DialogWindow : Window
{
    public DialogWindow()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
