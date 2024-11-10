using System.Windows;
using System.Windows.Controls;

namespace CYR.Dialog
{
    /// <summary>
    /// Interaktionslogik für ErrorDialogView.xaml
    /// </summary>
    public partial class ErrorDialogView : UserControl
    {
        public ErrorDialogView()
        {
            InitializeComponent();
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
}
