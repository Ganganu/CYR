using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.Dialog;

/// <summary>
/// Interaktionslogik für Notification.xaml
/// </summary>
public partial class Notification : UserControl
{
    public Notification()
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

