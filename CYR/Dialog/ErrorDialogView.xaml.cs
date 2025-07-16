using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.Dialog;

/// <summary>
/// Interaktionslogik für ErrorDialogView.xaml
/// </summary>
public partial class ErrorDialogView : UserControl
{
    public ErrorDialogView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var window = this.Parent as Window;
        window.DialogResult = true;
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        var window = this.Parent as Window;
        window.DialogResult = false;
    }
}
