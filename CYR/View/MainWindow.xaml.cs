using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace CYR;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {        
     btnDashboard.IsChecked = true;   
    }
}