using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.Dashboard.DashboardViews;

/// <summary>
/// Interaktionslogik für StatisticChartView.xaml
/// </summary>
public partial class StatisticChartView : UserControl
{
    public StatisticChartView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
