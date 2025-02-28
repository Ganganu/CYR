using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace CYR.CustomControls
{
    public partial class FlatExpander : UserControl
    {
        public FlatExpander()
        {
            InitializeComponent();
        }
        private void SPCard_Click(object sender, RoutedEventArgs e)
        {
            cbDropDown.IsChecked = !cbDropDown.IsChecked;
        }

        private void cbDropDown_Checked(object sender, RoutedEventArgs e)
        {
            ContentBorder.Visibility = Visibility.Visible;
        }

        private void cbDropDown_Unchecked(object sender, RoutedEventArgs e)
        {
            ContentBorder.Visibility = Visibility.Collapsed;
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(object),
                typeof(FlatExpander),
                new PropertyMetadata(null, OnContentChanged));

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlatExpander expander)
            {
                expander.ExpanderContent.Content = e.NewValue;
            }
        }

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register(
                nameof(HeaderText),
                typeof(string),
                typeof(FlatExpander),
                new PropertyMetadata("Header", OnHeaderTextChanged));

        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        private static void OnHeaderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlatExpander expander)
            {

            }
        }

        // DependencyProperty for the header icon
        public static readonly DependencyProperty HeaderIconProperty =
            DependencyProperty.Register(
                nameof(HeaderIcon),
                typeof(PackIconKind),
                typeof(FlatExpander),
                new PropertyMetadata(PackIconKind.Person, OnHeaderIconChanged));

        public PackIconKind HeaderIcon
        {
            get => (PackIconKind)GetValue(HeaderIconProperty);
            set => SetValue(HeaderIconProperty, value);
        }

        private static void OnHeaderIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlatExpander expander)
            {
                // Button Icon updaten, wenn das nötig ist
            }
        }

        // DependencyProperty for the expanded state
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(
                nameof(IsExpanded),
                typeof(bool),
                typeof(FlatExpander),
                new PropertyMetadata(false, OnIsExpandedChanged));

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlatExpander expander)
            {
                expander.ContentBorder.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        public Brush IconForeground
        {
            get { return (Brush)GetValue(IconForegroundProperty); }
            set { SetValue(IconForegroundProperty, value); }
        }

        public static readonly DependencyProperty IconForegroundProperty =
            DependencyProperty.Register("MyProperty", typeof(Brush), typeof(FlatExpander), new PropertyMetadata(Brushes.White));


    }
}
