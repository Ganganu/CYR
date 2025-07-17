using System.Windows;
using System.Windows.Controls;

namespace CYR.Dialog;

/// <summary>
/// Interaktionslogik für SaveCommentsDialogView.xaml
/// </summary>
public partial class SaveCommentsDialogView : UserControl
{
    public SaveCommentsDialogView()
    {
        InitializeComponent();
    }

    private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var window = this.Parent as Window;
        window.DialogResult = true;
    }

    private void btnAccept_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var window = this.Parent as Window;
        window.DialogResult = false;
    }
}
