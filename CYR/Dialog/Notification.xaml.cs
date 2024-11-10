using System.Windows;
using System.Windows.Controls;

namespace CYR.Dialog
{
    /// <summary>
    /// Interaktionslogik für Notification.xaml
    /// </summary>
    public partial class Notification : UserControl
    {
        public Notification()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = this.Parent as Window;
            window.DialogResult = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var window = this.Parent as Window;
            window.DialogResult = true;
        }
    }
}
