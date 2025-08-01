using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.Dashboard.DashboardViews;

/// <summary>
/// Interaktionslogik für StatisticOverviewView.xaml
/// </summary>
public partial class StatisticOverviewView : UserControl
{
    public StatisticOverviewView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
