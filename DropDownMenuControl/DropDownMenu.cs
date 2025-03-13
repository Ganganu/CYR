using System.Windows;
using System.Windows.Controls;

namespace DropDownMenuControl
{
    public class DropDownMenu : ContentControl
    {
        static DropDownMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownMenu), new FrameworkPropertyMetadata(typeof(DropDownMenu)));
        }



        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DropDownMenu), new PropertyMetadata(false));
    }
}