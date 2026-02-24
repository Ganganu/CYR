using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.Dialog;

public partial class InputDialog : UserControl
{
    public InputDialog()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
        InvoiceNumber.Focus();
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
