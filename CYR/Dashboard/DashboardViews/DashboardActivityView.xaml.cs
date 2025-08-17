using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.Dashboard.DashboardViews
{
    /// <summary>
    /// Interaktionslogik für DashboardUserView.xaml
    /// </summary>
    public partial class DashboardActivityView : UserControl
    {
        public DashboardActivityView()
        {
            InitializeComponent();
            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);

        }
    }
}
