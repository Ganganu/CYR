using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using Microsoft.Xaml.Behaviors.Core;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace CYR;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IRecipient<NavigateBackSource>, IRecipient<NavigateToInvoiceEvent>
{
    public MainWindow()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.RegisterAll(this);
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }

    /// <summary>
    /// Gesendet von einem UserControl, wenn aud Dashboard navigiert wird, um die Farbe des Buttons zu ändern.
    /// </summary>
    /// <param name="message"></param>
    public void Receive(NavigateBackSource message)
    {
        btnDashboard.IsChecked = false;
        btnDashboard.IsChecked = true;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        btnDashboard.IsChecked = true;
    }

    private void snackbarButton_Click(object sender, RoutedEventArgs e)
    {
        snackbarButton.Hide();
    }

    /// <summary>
    /// Gesendet wenn zu Invoice navigiert wird
    /// </summary>
    /// <param name="message"></param>
    public void Receive(NavigateToInvoiceEvent message)
    {
        invoiceIconButton.IsChecked = false;
        invoiceIconButton.IsChecked = true;
    }
}