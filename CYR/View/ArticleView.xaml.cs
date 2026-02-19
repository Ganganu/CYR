using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.View;

/// <summary>
/// Interaktionslogik für ArticleView.xaml
/// </summary>
public partial class ArticleView : UserControl
{
    public ArticleView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
