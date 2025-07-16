using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CYR.View;

/// <summary>
/// Interaktionslogik für CreateNewArticleView.xaml
/// </summary>
public partial class CreateNewArticleView : UserControl
{
    public CreateNewArticleView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
    }
}
