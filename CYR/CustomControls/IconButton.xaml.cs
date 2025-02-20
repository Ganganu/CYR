using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CYR.CustomControls
{
    /// <summary>
    /// Interaktionslogik für IconButton.xaml
    /// </summary>
    public partial class IconButton : UserControl
    {
        public IconButton()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty CommandProperty =
    DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(IconButton), new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(IconButton), new PropertyMetadata("Kunden"));

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(nameof(IconKind), typeof(PackIconKind), typeof(IconButton), new PropertyMetadata(PackIconKind.Person));

        public PackIconKind IconKind
        {
            get => (PackIconKind)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }
    }
}
