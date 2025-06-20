using System.Windows;
using System.Windows.Controls;

namespace CYR.Dialog
{
    /// <summary>
    /// Interaktionslogik für ItemsListView.xaml
    /// </summary>
    public partial class ItemsListView : UserControl
    {
        public ItemsListView()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var window = this.Parent as Window;
            window.DialogResult = false;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            var window = this.Parent as Window;
            window.DialogResult = false;
        }
    }
}
